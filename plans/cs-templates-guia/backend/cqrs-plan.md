# Plan CQRS: Templates y Guia para Artistas Noveles

**Fecha:** 2026-02-15
**Modulo:** Crowdfunding
**Feature:** cs-templates-guia (US-CS-01)

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Request | Response |
|-----------|------|---------|----------|
| Listar templates activos | Query | GetPlantillasProyectoQuery | ServiceResponse&lt;List&lt;PlantillaProyectoListDto&gt;&gt; |
| Detalle de template | Query | GetPlantillaProyectoByIdQuery | ServiceResponse&lt;PlantillaProyectoDto&gt; |
| Generar necesidades | Command | GenerarNecesidadesDesdeTemplateCommand | ServiceResponse&lt;GenerarNecesidadesResultDto&gt; |
| Listar roles profesionales | Query | GetRolesProfesionalesQuery | ServiceResponse&lt;List&lt;RolProfesionalConCategoriaDto&gt;&gt; |
| Listar categorias de roles | Query | GetCategoriasRolQuery | ServiceResponse&lt;List&lt;CategoriaRolDto&gt;&gt; |

---

## 2. Queries

### 2.1 GetPlantillasProyectoQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Templates/Queries/GetPlantillasProyectoQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

#### Query
| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| (Sin parametros) | - | Lista todas las plantillas activas |

**Implementa:** `IRequest<ServiceResponse<List<PlantillaProyectoListDto>>>`

#### Handler

**Dependencias:**
- `IPlantillaProyectoService` - Para obtener plantillas activas
- `IMapper` - Para mapear PlantillaProyecto -> PlantillaProyectoListDto
- `ILogger<GetPlantillasProyectoQueryHandler>` - Para logging

**Flujo:**
1. Llamar `_service.GetAllActivosAsync(ct)`
2. Service retorna `List<PlantillaProyecto>` (entidades sin navegaciones)
3. Para cada plantilla, calcular totales via Include de necesidades (hacer query adicional o usar projection)
4. Mapear entidades -> DTOs con AutoMapper (incluye calculos de PrecioMinTotal, PrecioMaxTotal, CantidadNecesidades, Fases)
5. Retornar ServiceResponse con lista de DTOs
6. Try-catch con logging para errores inesperados

**Implementacion:**
```csharp
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Infra.Interfaces;

namespace WePlayRises.Crowdfunding.Application.Features.Templates.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
/// <summary>
/// Query para listar todas las plantillas de proyecto activas
/// Retorna resumen con precio total, cantidad de necesidades y fases
/// </summary>
public class GetPlantillasProyectoQuery : IRequest<ServiceResponse<List<PlantillaProyectoListDto>>>
{
    // No parameters - returns all active templates
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetPlantillasProyectoQueryHandler : IRequestHandler<GetPlantillasProyectoQuery, ServiceResponse<List<PlantillaProyectoListDto>>>
{
    private readonly IPlantillaProyectoService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPlantillasProyectoQueryHandler> _logger;

    public GetPlantillasProyectoQueryHandler(
        IPlantillaProyectoService service,
        IMapper mapper,
        ILogger<GetPlantillasProyectoQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<List<PlantillaProyectoListDto>>> Handle(
        GetPlantillasProyectoQuery request,
        CancellationToken ct)
    {
        try
        {
            // 1. Obtener plantillas activas (Service usa cache)
            var plantillas = await _service.GetAllActivosAsync(ct);

            // 2. Para calcular totales necesitamos cargar necesidades
            // El Service deberia tener un metodo GetAllActivosWithNecesidadesAsync()
            // o usamos projection en el mapper
            var plantillasWithNecesidades = new List<PlantillaProyecto>();
            foreach (var plantilla in plantillas)
            {
                var plantillaCompleta = await _service.GetByIdWithNecesidadesAsync(plantilla.Id, ct);
                if (plantillaCompleta != null)
                {
                    plantillasWithNecesidades.Add(plantillaCompleta);
                }
            }

            // 3. Mapear a DTOs (AutoMapper calcula totales)
            var dtos = _mapper.Map<List<PlantillaProyectoListDto>>(plantillasWithNecesidades);

            // 4. Retornar respuesta exitosa
            return new ServiceResponse<List<PlantillaProyectoListDto>>
            {
                Data = dtos,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Plantillas obtenidas exitosamente",
                        ErrorCode = ServiceResponseMessageType.Success
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener plantillas de proyecto");
            return new ServiceResponse<List<PlantillaProyectoListDto>>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Error inesperado al obtener plantillas",
                        ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
                    }
                }
            };
        }
    }
}
```

---

### 2.2 GetPlantillaProyectoByIdQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Templates/Queries/GetPlantillaProyectoByIdQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

#### Query
| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | ID de la plantilla |

**Implementa:** `IRequest<ServiceResponse<PlantillaProyectoDto>>`

#### Handler

**Dependencias:**
- `IPlantillaProyectoService` - Para obtener plantilla con necesidades
- `IMapper` - Para mapear PlantillaProyecto -> PlantillaProyectoDto
- `IValidator<GetPlantillaProyectoByIdQuery>` - Para validar ID
- `ILogger<GetPlantillaProyectoByIdQueryHandler>` - Para logging

**Flujo:**
1. Validar request con FluentValidation (ID no vacio)
2. Si invalido, retornar ServiceResponse con errores de validacion
3. Llamar `_service.GetByIdWithNecesidadesAsync(id, ct)` (incluye Necesidades, RolProfesional, CategoriaRol)
4. Si null, retornar NOT_FOUND (ErrorCode 2006)
5. Mapear entidad -> DTO (AutoMapper calcula resumen con totales y counts por prioridad)
6. Retornar ServiceResponse con DTO
7. Try-catch con logging para errores inesperados

**Implementacion:**
```csharp
using MediatR;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Infra.Interfaces;

namespace WePlayRises.Crowdfunding.Application.Features.Templates.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
/// <summary>
/// Query para obtener detalle completo de una plantilla con necesidades
/// </summary>
public class GetPlantillaProyectoByIdQuery : IRequest<ServiceResponse<PlantillaProyectoDto>>
{
    public Guid Id { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetPlantillaProyectoByIdQueryHandler : IRequestHandler<GetPlantillaProyectoByIdQuery, ServiceResponse<PlantillaProyectoDto>>
{
    private readonly IPlantillaProyectoService _service;
    private readonly IMapper _mapper;
    private readonly IValidator<GetPlantillaProyectoByIdQuery> _validator;
    private readonly ILogger<GetPlantillaProyectoByIdQueryHandler> _logger;

    public GetPlantillaProyectoByIdQueryHandler(
        IPlantillaProyectoService service,
        IMapper mapper,
        IValidator<GetPlantillaProyectoByIdQuery> validator,
        ILogger<GetPlantillaProyectoByIdQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PlantillaProyectoDto>> Handle(
        GetPlantillaProyectoByIdQuery request,
        CancellationToken ct)
    {
        try
        {
            // 1. Validar request
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<PlantillaProyectoDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Obtener plantilla con necesidades (Service usa cache)
            var plantilla = await _service.GetByIdWithNecesidadesAsync(request.Id, ct);

            // 3. Validar que existe
            if (plantilla == null)
            {
                return new ServiceResponse<PlantillaProyectoDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Plantilla no encontrada",
                            ErrorCode = ServiceResponseMessageType.NotFound_PlantillaProyecto
                        }
                    }
                };
            }

            // 4. Mapear a DTO (AutoMapper calcula resumen)
            var dto = _mapper.Map<PlantillaProyectoDto>(plantilla);

            // 5. Retornar respuesta exitosa
            return new ServiceResponse<PlantillaProyectoDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Plantilla obtenida exitosamente",
                        ErrorCode = ServiceResponseMessageType.Success
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener plantilla {PlantillaId}", request.Id);
            return new ServiceResponse<PlantillaProyectoDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Error inesperado al obtener plantilla",
                        ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
                    }
                }
            };
        }
    }
}
```

---

### 2.3 GetRolesProfesionalesQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Maestras/Queries/GetRolesProfesionalesQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

#### Query
| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| (Sin parametros) | - | Lista todos los roles activos con categoria |

**Implementa:** `IRequest<ServiceResponse<List<RolProfesionalConCategoriaDto>>>`

#### Handler

**Dependencias:**
- `IRolProfesionalService` - Para obtener roles activos con categoria
- `IMapper` - Para mapear MaestraRolProfesional -> RolProfesionalConCategoriaDto
- `ILogger<GetRolesProfesionalesQueryHandler>` - Para logging

**Flujo:**
1. Llamar `_service.GetAllActivosAsync(ct)` (incluye CategoriaRol)
2. Service retorna `List<MaestraRolProfesional>` con navegacion a categoria
3. Mapear entidades -> DTOs con AutoMapper
4. Retornar ServiceResponse con lista de DTOs
5. Try-catch con logging para errores inesperados

**Implementacion:**
```csharp
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Infra.Interfaces;

namespace WePlayRises.Crowdfunding.Application.Features.Maestras.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
/// <summary>
/// Query para obtener catalogo de roles profesionales con categoria
/// </summary>
public class GetRolesProfesionalesQuery : IRequest<ServiceResponse<List<RolProfesionalConCategoriaDto>>>
{
    // No parameters - returns all active roles
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetRolesProfesionalesQueryHandler : IRequestHandler<GetRolesProfesionalesQuery, ServiceResponse<List<RolProfesionalConCategoriaDto>>>
{
    private readonly IRolProfesionalService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetRolesProfesionalesQueryHandler> _logger;

    public GetRolesProfesionalesQueryHandler(
        IRolProfesionalService service,
        IMapper mapper,
        ILogger<GetRolesProfesionalesQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<List<RolProfesionalConCategoriaDto>>> Handle(
        GetRolesProfesionalesQuery request,
        CancellationToken ct)
    {
        try
        {
            // 1. Obtener roles activos con categoria (Service usa cache largo)
            var roles = await _service.GetAllActivosAsync(ct);

            // 2. Mapear a DTOs
            var dtos = _mapper.Map<List<RolProfesionalConCategoriaDto>>(roles);

            // 3. Retornar respuesta exitosa
            return new ServiceResponse<List<RolProfesionalConCategoriaDto>>
            {
                Data = dtos,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Roles profesionales obtenidos exitosamente",
                        ErrorCode = ServiceResponseMessageType.Success
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener roles profesionales");
            return new ServiceResponse<List<RolProfesionalConCategoriaDto>>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Error inesperado al obtener roles",
                        ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
                    }
                }
            };
        }
    }
}
```

---

### 2.4 GetCategoriasRolQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Maestras/Queries/GetCategoriasRolQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

#### Query
| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| (Sin parametros) | - | Lista todas las categorias de roles |

**Implementa:** `IRequest<ServiceResponse<List<CategoriaRolDto>>>`

#### Handler

**Dependencias:**
- `ICategoriaRolService` - Para obtener categorias ordenadas
- `IMapper` - Para mapear MaestraCategoriaRol -> CategoriaRolDto
- `ILogger<GetCategoriasRolQueryHandler>` - Para logging

**Flujo:**
1. Llamar `_service.GetAllAsync(ct)` (ordenadas por Orden)
2. Service retorna `List<MaestraCategoriaRol>`
3. Mapear entidades -> DTOs con AutoMapper
4. Retornar ServiceResponse con lista de DTOs
5. Try-catch con logging para errores inesperados

**Implementacion:**
```csharp
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Infra.Interfaces;

namespace WePlayRises.Crowdfunding.Application.Features.Maestras.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
/// <summary>
/// Query para obtener categorias de roles profesionales
/// </summary>
public class GetCategoriasRolQuery : IRequest<ServiceResponse<List<CategoriaRolDto>>>
{
    // No parameters - returns all categories
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetCategoriasRolQueryHandler : IRequestHandler<GetCategoriasRolQuery, ServiceResponse<List<CategoriaRolDto>>>
{
    private readonly ICategoriaRolService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCategoriasRolQueryHandler> _logger;

    public GetCategoriasRolQueryHandler(
        ICategoriaRolService service,
        IMapper mapper,
        ILogger<GetCategoriasRolQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<List<CategoriaRolDto>>> Handle(
        GetCategoriasRolQuery request,
        CancellationToken ct)
    {
        try
        {
            // 1. Obtener categorias (Service usa cache largo)
            var categorias = await _service.GetAllAsync(ct);

            // 2. Mapear a DTOs
            var dtos = _mapper.Map<List<CategoriaRolDto>>(categorias);

            // 3. Retornar respuesta exitosa
            return new ServiceResponse<List<CategoriaRolDto>>
            {
                Data = dtos,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Categorias obtenidas exitosamente",
                        ErrorCode = ServiceResponseMessageType.Success
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorias de roles");
            return new ServiceResponse<List<CategoriaRolDto>>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Error inesperado al obtener categorias",
                        ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
                    }
                }
            };
        }
    }
}
```

---

## 3. Commands

### 3.1 GenerarNecesidadesDesdeTemplateCommand

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Templates/Commands/GenerarNecesidadesDesdeTemplateCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

#### Command
| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| PlantillaId | Guid | ID de la plantilla seleccionada |
| ProyectoArtisticoId | Guid | ID del proyecto artistico del artista |
| NecesidadesSeleccionadas | List&lt;NecesidadSeleccionadaDto&gt; | Items seleccionados con presupuestos personalizados |
| UserId | string? | ID del usuario autenticado (inyectado desde claim JWT) |

**Implementa:** `IRequest<ServiceResponse<GenerarNecesidadesResultDto>>`

**DTO anidado:**
```csharp
public class NecesidadSeleccionadaDto
{
    public Guid PlantillaNecesidadId { get; set; }
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int MonedaId { get; set; }
}
```

#### Handler

**Dependencias:**
- `IPlantillaProyectoService` - Para obtener plantilla y validar existencia
- `IPlantillaProyectoNecesidadService` - Para obtener necesidades seleccionadas
- `IProyectoArtisticoService` - Para validar proyecto y ownership
- `INecesidadCrowdsourcingService` - Para crear necesidades masivamente
- `IMapper` - Para mapear PlantillaProyectoNecesidad -> NecesidadCrowdsourcing
- `IValidator<GenerarNecesidadesDesdeTemplateCommand>` - Para validar request
- `ILogger<GenerarNecesidadesDesdeTemplateCommandHandler>` - Para logging

**Flujo:**
1. **Validar request** con FluentValidation (IDs, presupuestos, min 1 necesidad)
2. **Validar ownership:**
   - Obtener proyecto via `_proyectoService.GetByIdAsync(request.ProyectoArtisticoId, ct)`
   - Comparar `proyecto.ArtistaId` con `request.UserId` (del claim JWT)
   - Si no coinciden, retornar FORBIDDEN (ErrorCode 3002)
3. **Obtener plantilla** con necesidades via `_plantillaService.GetByIdWithNecesidadesAsync(request.PlantillaId, ct)`
4. **Validar que todas las PlantillaNecesidadId existen** en la plantilla seleccionada
5. **Crear entidades NecesidadCrowdsourcing:**
   - Para cada item en `NecesidadesSeleccionadas`, crear una `NecesidadCrowdsourcing`
   - Copiar datos de `PlantillaProyectoNecesidad` (Titulo, Descripcion, Fase, RolProfesionalId)
   - Usar presupuestos personalizados del request (si existen) o valores orientativos de la plantilla
   - Establecer `Estado = "Abierta"`, `FechaCreacion = DateTime.UtcNow`, `ProyectoArtisticoId = request.ProyectoArtisticoId`
6. **Persistir en bulk** via `_necesidadService.CreateManyAsync(necesidades, ct)` (transaccion atomica)
7. **Calcular totales:**
   - `PresupuestoTotalMin` = Suma de todos los `PresupuestoMin`
   - `PresupuestoTotalMax` = Suma de todos los `PresupuestoMax`
8. **Retornar resultado:**
   - `NecesidadesCreadas` = cantidad de necesidades creadas
   - `NecesidadIds` = lista de GUIDs generados
   - `PresupuestoTotalMin`, `PresupuestoTotalMax`, `Moneda`
9. Try-catch con logging para errores inesperados

**Implementacion:**
```csharp
using MediatR;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;
using WePlayRises.Crowdfunding.Infra.Interfaces;

namespace WePlayRises.Crowdfunding.Application.Features.Templates.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
/// <summary>
/// Command para generar necesidades de crowdsourcing desde una plantilla
/// Crea multiples NecesidadCrowdsourcing en una transaccion atomica
/// </summary>
public class GenerarNecesidadesDesdeTemplateCommand : IRequest<ServiceResponse<GenerarNecesidadesResultDto>>
{
    public Guid PlantillaId { get; set; }
    public Guid ProyectoArtisticoId { get; set; }
    public List<NecesidadSeleccionadaDto> NecesidadesSeleccionadas { get; set; } = new();

    /// <summary>
    /// ID del usuario autenticado (se inyecta desde claim JWT en el Handler)
    /// NO viene en el request body - se obtiene de HttpContext
    /// </summary>
    public string? UserId { get; set; }
}

/// <summary>
/// DTO para item de necesidad seleccionada con presupuesto personalizado
/// </summary>
public class NecesidadSeleccionadaDto
{
    public Guid PlantillaNecesidadId { get; set; }
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int MonedaId { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GenerarNecesidadesDesdeTemplateCommandHandler : IRequestHandler<GenerarNecesidadesDesdeTemplateCommand, ServiceResponse<GenerarNecesidadesResultDto>>
{
    private readonly IPlantillaProyectoService _plantillaService;
    private readonly IPlantillaProyectoNecesidadService _plantillaNecesidadService;
    private readonly IProyectoArtisticoService _proyectoService;
    private readonly INecesidadCrowdsourcingService _necesidadService;
    private readonly IMapper _mapper;
    private readonly IValidator<GenerarNecesidadesDesdeTemplateCommand> _validator;
    private readonly ILogger<GenerarNecesidadesDesdeTemplateCommandHandler> _logger;

    public GenerarNecesidadesDesdeTemplateCommandHandler(
        IPlantillaProyectoService plantillaService,
        IPlantillaProyectoNecesidadService plantillaNecesidadService,
        IProyectoArtisticoService proyectoService,
        INecesidadCrowdsourcingService necesidadService,
        IMapper mapper,
        IValidator<GenerarNecesidadesDesdeTemplateCommand> validator,
        ILogger<GenerarNecesidadesDesdeTemplateCommandHandler> logger)
    {
        _plantillaService = plantillaService ?? throw new ArgumentNullException(nameof(plantillaService));
        _plantillaNecesidadService = plantillaNecesidadService ?? throw new ArgumentNullException(nameof(plantillaNecesidadService));
        _proyectoService = proyectoService ?? throw new ArgumentNullException(nameof(proyectoService));
        _necesidadService = necesidadService ?? throw new ArgumentNullException(nameof(necesidadService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<GenerarNecesidadesResultDto>> Handle(
        GenerarNecesidadesDesdeTemplateCommand request,
        CancellationToken ct)
    {
        try
        {
            // 1. Validar request
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<GenerarNecesidadesResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Validar ownership del proyecto (CRITICO)
            if (string.IsNullOrEmpty(request.UserId))
            {
                return new ServiceResponse<GenerarNecesidadesResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Usuario no autenticado",
                            ErrorCode = ServiceResponseMessageType.Auth_Unauthorized
                        }
                    }
                };
            }

            var proyecto = await _proyectoService.GetByIdAsync(request.ProyectoArtisticoId, ct);
            if (proyecto == null)
            {
                return new ServiceResponse<GenerarNecesidadesResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Proyecto artistico no encontrado",
                            ErrorCode = ServiceResponseMessageType.NotFound_ProyectoArtistico
                        }
                    }
                };
            }

            // Validar que el proyecto pertenece al artista autenticado
            if (proyecto.ArtistaId.ToString() != request.UserId)
            {
                _logger.LogWarning(
                    "Intento de generar necesidades para proyecto {ProyectoId} por usuario {UserId} no autorizado",
                    request.ProyectoArtisticoId, request.UserId);

                return new ServiceResponse<GenerarNecesidadesResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "No tienes permiso para modificar este proyecto",
                            ErrorCode = ServiceResponseMessageType.Auth_Forbidden
                        }
                    }
                };
            }

            // 3. Obtener plantilla con necesidades
            var plantilla = await _plantillaService.GetByIdWithNecesidadesAsync(request.PlantillaId, ct);
            if (plantilla == null)
            {
                return new ServiceResponse<GenerarNecesidadesResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Plantilla no encontrada",
                            ErrorCode = ServiceResponseMessageType.NotFound_PlantillaProyecto
                        }
                    }
                };
            }

            // 4. Validar que todas las PlantillaNecesidadId existen en la plantilla
            var plantillaNecesidadIds = plantilla.Necesidades.Select(n => n.Id).ToHashSet();
            var necesidadesSeleccionadasIds = request.NecesidadesSeleccionadas.Select(n => n.PlantillaNecesidadId).ToList();
            var necesidadesInvalidas = necesidadesSeleccionadasIds.Where(id => !plantillaNecesidadIds.Contains(id)).ToList();

            if (necesidadesInvalidas.Any())
            {
                _logger.LogWarning(
                    "Intento de generar necesidades con IDs invalidos: {InvalidIds} para plantilla {PlantillaId}",
                    string.Join(", ", necesidadesInvalidas), request.PlantillaId);

                return new ServiceResponse<GenerarNecesidadesResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Una o mas necesidades de plantilla no fueron encontradas",
                            ErrorCode = ServiceResponseMessageType.NotFound_PlantillaNecesidad
                        }
                    }
                };
            }

            // 5. Crear entidades NecesidadCrowdsourcing
            var necesidadesACrear = new List<NecesidadCrowdsourcing>();
            var presupuestoTotalMin = 0m;
            var presupuestoTotalMax = 0m;
            var monedaId = 1; // EUR por defecto

            foreach (var itemSeleccionado in request.NecesidadesSeleccionadas)
            {
                // Buscar la plantilla necesidad original
                var plantillaNecesidad = plantilla.Necesidades
                    .FirstOrDefault(n => n.Id == itemSeleccionado.PlantillaNecesidadId);

                if (plantillaNecesidad == null) continue; // Ya validado arriba, pero por seguridad

                // Crear nueva necesidad crowdsourcing
                var necesidad = new NecesidadCrowdsourcing
                {
                    Id = Guid.NewGuid(),
                    ProyectoArtisticoId = request.ProyectoArtisticoId,
                    Titulo = plantillaNecesidad.Titulo,
                    Descripcion = plantillaNecesidad.Descripcion,
                    Fase = plantillaNecesidad.Fase,
                    RolProfesionalId = plantillaNecesidad.RolProfesionalId,

                    // Usar presupuestos personalizados si existen, sino usar orientativos
                    PresupuestoMin = itemSeleccionado.PresupuestoMin ?? plantillaNecesidad.PrecioMinOrientativo,
                    PresupuestoMax = itemSeleccionado.PresupuestoMax ?? plantillaNecesidad.PrecioMaxOrientativo,
                    MonedaId = itemSeleccionado.MonedaId,

                    Estado = "Abierta", // Estado inicial
                    FechaCreacion = DateTime.UtcNow,
                    FechaActualizacion = DateTime.UtcNow
                };

                necesidadesACrear.Add(necesidad);

                // Acumular totales
                presupuestoTotalMin += necesidad.PresupuestoMin ?? 0;
                presupuestoTotalMax += necesidad.PresupuestoMax ?? 0;
                monedaId = necesidad.MonedaId; // Asumir misma moneda para todas
            }

            // 6. Persistir en bulk (transaccion atomica)
            var necesidadIds = await _necesidadService.CreateManyAsync(necesidadesACrear, ct);

            _logger.LogInformation(
                "Generadas {Count} necesidades desde plantilla {PlantillaId} para proyecto {ProyectoId}",
                necesidadIds.Count, request.PlantillaId, request.ProyectoArtisticoId);

            // 7. Retornar resultado
            return new ServiceResponse<GenerarNecesidadesResultDto>
            {
                Data = new GenerarNecesidadesResultDto
                {
                    NecesidadesCreadas = necesidadIds.Count,
                    NecesidadIds = necesidadIds,
                    PresupuestoTotalMin = presupuestoTotalMin,
                    PresupuestoTotalMax = presupuestoTotalMax,
                    Moneda = monedaId
                },
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Necesidades generadas exitosamente a partir de la plantilla",
                        ErrorCode = ServiceResponseMessageType.Created
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error al generar necesidades desde plantilla {PlantillaId} para proyecto {ProyectoId}",
                request.PlantillaId, request.ProyectoArtisticoId);

            return new ServiceResponse<GenerarNecesidadesResultDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Error inesperado al generar necesidades",
                        ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
                    }
                }
            };
        }
    }
}
```

---

## 4. Validators

### 4.1 GetPlantillaProyectoByIdValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Templates/Validators/GetPlantillaProyectoByIdValidator.cs`

**Reglas:**
| Campo | Regla | Mensaje | ErrorCode (Constant) |
|-------|-------|---------|----------------------|
| Id | NotEmpty | "El ID de la plantilla es obligatorio" | `ServiceResponseMessageType.Validation_Required` |

**Implementacion:**
```csharp
using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Templates.Queries;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Templates.Validators;

/// <summary>
/// Validador para GetPlantillaProyectoByIdQuery
/// </summary>
public class GetPlantillaProyectoByIdValidator : AbstractValidator<GetPlantillaProyectoByIdQuery>
{
    public GetPlantillaProyectoByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("El ID de la plantilla es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
```

---

### 4.2 GenerarNecesidadesDesdeTemplateValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Templates/Validators/GenerarNecesidadesDesdeTemplateValidator.cs`

**Reglas sincronas:**
| Campo | Regla | Mensaje | ErrorCode (Constant) |
|-------|-------|---------|----------------------|
| PlantillaId | NotEmpty | "El ID de la plantilla es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| ProyectoArtisticoId | NotEmpty | "El ID del proyecto artistico es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| NecesidadesSeleccionadas | NotEmpty | "Debe seleccionar al menos una necesidad" | `ServiceResponseMessageType.Validation_Required` |
| NecesidadesSeleccionadas | Count > 0 | "Debe seleccionar al menos una necesidad" | `ServiceResponseMessageType.Validation_Required` |
| NecesidadesSeleccionadas[].PlantillaNecesidadId | NotEmpty | "El ID de la necesidad es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| NecesidadesSeleccionadas[].PresupuestoMin | >= 0 (when present) | "El presupuesto minimo no puede ser negativo" | `ServiceResponseMessageType.Validation_InvalidRange` |
| NecesidadesSeleccionadas[].PresupuestoMax | >= PresupuestoMin (when both present) | "El presupuesto maximo debe ser mayor o igual al minimo" | `ServiceResponseMessageType.Validation_InvalidRange` |
| NecesidadesSeleccionadas[].MonedaId | > 0 | "La moneda es obligatoria" | `ServiceResponseMessageType.Validation_Required` |

**Reglas asincronas (negocio):**
- Plantilla existe y esta activa (via `IPlantillaProyectoService`)
- ProyectoArtistico existe (via `IProyectoArtisticoService`)
- **NOTA:** Ownership NO se valida aqui (se hace en Handler porque requiere UserId del claim)

**Implementacion:**
```csharp
using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Templates.Commands;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Infra.Interfaces;

namespace WePlayRises.Crowdfunding.Application.Features.Templates.Validators;

/// <summary>
/// Validador para GenerarNecesidadesDesdeTemplateCommand
/// Valida presupuestos, plantilla, proyecto
/// NOTA: Ownership se valida en el Handler (requiere UserId del claim)
/// </summary>
public class GenerarNecesidadesDesdeTemplateValidator : AbstractValidator<GenerarNecesidadesDesdeTemplateCommand>
{
    private readonly IPlantillaProyectoService _plantillaService;
    private readonly IProyectoArtisticoService _proyectoService;

    public GenerarNecesidadesDesdeTemplateValidator(
        IPlantillaProyectoService plantillaService,
        IProyectoArtisticoService proyectoService)
    {
        _plantillaService = plantillaService ?? throw new ArgumentNullException(nameof(plantillaService));
        _proyectoService = proyectoService ?? throw new ArgumentNullException(nameof(proyectoService));

        // ========== Validaciones sincronas basicas ==========

        RuleFor(x => x.PlantillaId)
            .NotEmpty()
            .WithMessage("El ID de la plantilla es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ProyectoArtisticoId)
            .NotEmpty()
            .WithMessage("El ID del proyecto artistico es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.NecesidadesSeleccionadas)
            .NotEmpty()
            .WithMessage("Debe seleccionar al menos una necesidad")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .Must(x => x != null && x.Count > 0)
            .WithMessage("Debe seleccionar al menos una necesidad")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        // ========== Validaciones de cada item de necesidad ==========

        RuleForEach(x => x.NecesidadesSeleccionadas).ChildRules(item =>
        {
            item.RuleFor(x => x.PlantillaNecesidadId)
                .NotEmpty()
                .WithMessage("El ID de la necesidad es obligatorio")
                .WithErrorCode(ServiceResponseMessageType.Validation_Required);

            item.RuleFor(x => x.PresupuestoMin)
                .GreaterThanOrEqualTo(0)
                .When(x => x.PresupuestoMin.HasValue)
                .WithMessage("El presupuesto minimo no puede ser negativo")
                .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

            item.RuleFor(x => x.PresupuestoMax)
                .GreaterThanOrEqualTo(x => x.PresupuestoMin ?? 0)
                .When(x => x.PresupuestoMax.HasValue)
                .WithMessage("El presupuesto maximo debe ser mayor o igual al minimo")
                .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

            item.RuleFor(x => x.MonedaId)
                .GreaterThan(0)
                .WithMessage("La moneda es obligatoria")
                .WithErrorCode(ServiceResponseMessageType.Validation_Required);
        });

        // ========== Validaciones asincronas de negocio ==========

        RuleFor(x => x.PlantillaId)
            .MustAsync(PlantillaExisteYEstaActiva)
            .WithMessage("La plantilla no existe o no esta activa")
            .WithErrorCode(ServiceResponseMessageType.NotFound_PlantillaProyecto);

        RuleFor(x => x.ProyectoArtisticoId)
            .MustAsync(ProyectoExiste)
            .WithMessage("El proyecto artistico no existe")
            .WithErrorCode(ServiceResponseMessageType.NotFound_ProyectoArtistico);

        // NOTA: La validacion de ownership (proyecto.ArtistaId == UserId) se hace en el Handler
        // porque el Validator no tiene acceso al HttpContext ni a los claims JWT
    }

    private async Task<bool> PlantillaExisteYEstaActiva(Guid plantillaId, CancellationToken ct)
    {
        var plantilla = await _plantillaService.GetByIdAsync(plantillaId, ct);
        return plantilla != null && plantilla.Activo;
    }

    private async Task<bool> ProyectoExiste(Guid proyectoId, CancellationToken ct)
    {
        var proyecto = await _proyectoService.GetByIdAsync(proyectoId, ct);
        return proyecto != null;
    }
}
```

---

## 5. AutoMapper Profiles

### 5.1 PlantillaProyectoProfile

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/PlantillaProyectoProfile.cs`

**Mappings:**
| Source | Destination | Notas |
|--------|-------------|-------|
| PlantillaProyecto | PlantillaProyectoListDto | Calcula PrecioMinTotal, PrecioMaxTotal, CantidadNecesidades, Fases (distinct) |
| PlantillaProyecto | PlantillaProyectoDto | Include necesidades ordenadas, calcula resumen |
| PlantillaProyectoNecesidad | PlantillaProyectoNecesidadDto | Include RolProfesional anidado |

**Implementacion:**
```csharp
using AutoMapper;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Mapping;

/// <summary>
/// AutoMapper profile para PlantillaProyecto y PlantillaProyectoNecesidad
/// </summary>
public class PlantillaProyectoProfile : Profile
{
    public PlantillaProyectoProfile()
    {
        // ========== Entity -> DTO List (para galeria) ==========
        CreateMap<PlantillaProyecto, PlantillaProyectoListDto>()
            .ForMember(dest => dest.PrecioMinTotal,
                opt => opt.MapFrom(src => src.Necesidades.Sum(n => n.PrecioMinOrientativo ?? 0)))
            .ForMember(dest => dest.PrecioMaxTotal,
                opt => opt.MapFrom(src => src.Necesidades.Sum(n => n.PrecioMaxOrientativo ?? 0)))
            .ForMember(dest => dest.Moneda,
                opt => opt.MapFrom(src => src.Necesidades.FirstOrDefault() != null
                    ? src.Necesidades.First().MonedaId
                    : 1)) // EUR por defecto
            .ForMember(dest => dest.CantidadNecesidades,
                opt => opt.MapFrom(src => src.Necesidades.Count))
            .ForMember(dest => dest.Fases,
                opt => opt.MapFrom(src => src.Necesidades
                    .Select(n => n.Fase)
                    .Distinct()
                    .OrderBy(f => f)
                    .ToList()));

        // ========== Entity -> DTO Detail (con necesidades) ==========
        CreateMap<PlantillaProyecto, PlantillaProyectoDto>()
            .ForMember(dest => dest.Necesidades,
                opt => opt.MapFrom(src => src.Necesidades.OrderBy(n => n.Orden)))
            .ForMember(dest => dest.Resumen,
                opt => opt.MapFrom(src => new PlantillaResumenDto
                {
                    PrecioMinTotal = src.Necesidades.Sum(n => n.PrecioMinOrientativo ?? 0),
                    PrecioMaxTotal = src.Necesidades.Sum(n => n.PrecioMaxOrientativo ?? 0),
                    Moneda = src.Necesidades.FirstOrDefault() != null
                        ? src.Necesidades.First().MonedaId
                        : 1,
                    CantidadNecesidadesAlta = src.Necesidades.Count(n => n.Prioridad == "Alta"),
                    CantidadNecesidadesMedia = src.Necesidades.Count(n => n.Prioridad == "Media"),
                    CantidadNecesidadesBaja = src.Necesidades.Count(n => n.Prioridad == "Baja")
                }));

        // ========== PlantillaProyectoNecesidad -> DTO ==========
        CreateMap<PlantillaProyectoNecesidad, PlantillaProyectoNecesidadDto>()
            .ForMember(dest => dest.RolProfesional,
                opt => opt.MapFrom(src => src.RolProfesional));
    }
}
```

---

### 5.2 RolProfesionalProfile

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/RolProfesionalProfile.cs`

**Mappings:**
| Source | Destination | Notas |
|--------|-------------|-------|
| MaestraRolProfesional | RolProfesionalDto | Basico sin categoria anidada |
| MaestraRolProfesional | RolProfesionalConCategoriaDto | Con categoria anidada |

**Implementacion:**
```csharp
using AutoMapper;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Mapping;

/// <summary>
/// AutoMapper profile para MaestraRolProfesional
/// </summary>
public class RolProfesionalProfile : Profile
{
    public RolProfesionalProfile()
    {
        // Entity -> DTO basico (sin categoria anidada)
        CreateMap<MaestraRolProfesional, RolProfesionalDto>();

        // Entity -> DTO con categoria anidada
        CreateMap<MaestraRolProfesional, RolProfesionalConCategoriaDto>()
            .ForMember(dest => dest.CategoriaRol,
                opt => opt.MapFrom(src => src.CategoriaRol));
    }
}
```

---

### 5.3 CategoriaRolProfile

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/CategoriaRolProfile.cs`

**Mappings:**
| Source | Destination | Notas |
|--------|-------------|-------|
| MaestraCategoriaRol | CategoriaRolDto | Simple 1:1 mapping |

**Implementacion:**
```csharp
using AutoMapper;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Mapping;

/// <summary>
/// AutoMapper profile para MaestraCategoriaRol
/// </summary>
public class CategoriaRolProfile : Profile
{
    public CategoriaRolProfile()
    {
        // Entity -> DTO (simple 1:1 mapping)
        CreateMap<MaestraCategoriaRol, CategoriaRolDto>();
    }
}
```

---

## 6. Archivos a Crear

```
Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
├── Features/
│   ├── Templates/
│   │   ├── Queries/
│   │   │   ├── GetPlantillasProyectoQuery.cs          (NUEVO - Query + Handler)
│   │   │   └── GetPlantillaProyectoByIdQuery.cs       (NUEVO - Query + Handler)
│   │   ├── Commands/
│   │   │   └── GenerarNecesidadesDesdeTemplateCommand.cs (NUEVO - Command + Handler)
│   │   └── Validators/
│   │       ├── GetPlantillaProyectoByIdValidator.cs   (NUEVO)
│   │       └── GenerarNecesidadesDesdeTemplateValidator.cs (NUEVO)
│   │
│   └── Maestras/
│       └── Queries/
│           ├── GetRolesProfesionalesQuery.cs   (NUEVO - Query + Handler)
│           └── GetCategoriasRolQuery.cs        (NUEVO - Query + Handler)
│
└── Mapping/
    ├── PlantillaProyectoProfile.cs             (NUEVO)
    ├── RolProfesionalProfile.cs                (NUEVO)
    └── CategoriaRolProfile.cs                  (NUEVO)
```

**Total archivos:**
- 5 Queries/Commands (con handlers en mismo archivo)
- 2 Validators
- 3 AutoMapper Profiles

---

## 7. Patrones Importantes

### 7.1 Logica de Negocio en Handler

```
Handler contiene:
- Validacion con FluentValidation
- Logica de negocio (calculos, reglas, transformaciones)
- Validacion de ownership (proyecto pertenece al artista)
- Orquestacion de llamadas a services
- Creacion de entidades NecesidadCrowdsourcing desde PlantillaProyectoNecesidad

Service contiene:
- SOLO persistencia (CRUD via repository)
- Caching para evitar queries duplicados
- NO logica de negocio
```

### 7.2 ServiceResponse siempre con Constants

```csharp
// Exito - USAR CONSTANTS
return new ServiceResponse<T> {
    Data = result,
    Messages = new List<ServiceResponseMessage>
    {
        new() { Message = "Creado", ErrorCode = ServiceResponseMessageType.Created }
    }
};

// Error de validacion - Retornado desde validator
return new ServiceResponse<T> {
    Messages = validationResult.GetServiceResponseMessages()
};

// Error de negocio - USAR CONSTANTS
return new ServiceResponse<T> {
    Messages = new List<ServiceResponseMessage>
    {
        new() { Message = "...", ErrorCode = ServiceResponseMessageType.NotFound_PlantillaProyecto }
    }
};

// Error de ownership - USAR CONSTANTS
return new ServiceResponse<T> {
    Messages = new List<ServiceResponseMessage>
    {
        new() { Message = "...", ErrorCode = ServiceResponseMessageType.Auth_Forbidden }
    }
};
```

### 7.3 Caching Solo en Services (ADR-006)

```csharp
// Service usa RequestCacheService para evitar queries duplicados
public async Task<PlantillaProyecto?> GetByIdAsync(Guid id, CancellationToken ct)
{
    return await _requestCache.GetOrAddAsync(
        $"plantilla-proyecto:{id}",
        async () => await _repository.GetByIdAsync(id, ct));
}

// Beneficio: Validator y Handler comparten cache
// 1. Validator llama Service.GetByIdAsync(id) -> Cache MISS -> DB -> Cache
// 2. Handler llama Service.GetByIdAsync(id) -> Cache HIT -> Sin DB
```

### 7.4 Validacion de Ownership en Handler (NO en Validator)

```csharp
// CORRECTO - Handler valida ownership
public async Task<ServiceResponse<T>> Handle(Command request, CancellationToken ct)
{
    // Obtener UserId desde claim JWT (inyectado en request desde Controller)
    if (string.IsNullOrEmpty(request.UserId))
    {
        return new ServiceResponse<T> {
            Messages = new() { new() { ..., ErrorCode = ServiceResponseMessageType.Auth_Unauthorized } }
        };
    }

    var proyecto = await _proyectoService.GetByIdAsync(request.ProyectoArtisticoId, ct);
    if (proyecto.ArtistaId.ToString() != request.UserId)
    {
        return new ServiceResponse<T> {
            Messages = new() { new() { ..., ErrorCode = ServiceResponseMessageType.Auth_Forbidden } }
        };
    }

    // ... continuar flujo
}

// INCORRECTO - Validator NO tiene acceso a HttpContext ni claims
// NO se puede validar ownership en Validator
```

---

## 8. Checklist de Implementacion

### Queries + Handlers
- [ ] `GetPlantillasProyectoQuery` + Handler en MISMO archivo
- [ ] `GetPlantillaProyectoByIdQuery` + Handler en MISMO archivo
- [ ] `GetRolesProfesionalesQuery` + Handler en MISMO archivo
- [ ] `GetCategoriasRolQuery` + Handler en MISMO archivo
- [ ] Todos los handlers inyectan Service, Mapper, Logger (y Validator si aplica)
- [ ] **Constructor con `?? throw new ArgumentNullException` para TODAS las dependencias**
- [ ] Handlers NUNCA inyectan DbContext (usan Services)
- [ ] Try-catch con logging en todos los handlers
- [ ] **Handlers usan `ServiceResponseMessageType.X` constants (NO strings literales)**

### Commands + Handlers
- [ ] `GenerarNecesidadesDesdeTemplateCommand` + Handler en MISMO archivo
- [ ] Handler valida ownership del proyecto (proyecto.ArtistaId == UserId del claim)
- [ ] Handler crea entidades NecesidadCrowdsourcing desde PlantillaProyectoNecesidad
- [ ] Handler usa `INecesidadCrowdsourcingService.CreateManyAsync()` para bulk insert
- [ ] Handler calcula totales de presupuesto (min y max)
- [ ] Handler retorna `ServiceResponse<GenerarNecesidadesResultDto>` con IDs creados
- [ ] **Constructor con `?? throw new ArgumentNullException` para TODAS las dependencias**
- [ ] Try-catch con logging detallado

### Validators
- [ ] `GetPlantillaProyectoByIdValidator` en carpeta `Validators/` separada
- [ ] `GenerarNecesidadesDesdeTemplateValidator` en carpeta `Validators/` separada
- [ ] **Validators usan `ServiceResponseMessageType.X` constants (NO strings literales)**
- [ ] **Validators usan `.WithMessage()` Y `.WithErrorCode(ServiceResponseMessageType.X)`**
- [ ] Validators con validaciones asincronas (plantilla existe, proyecto existe)
- [ ] Validators NO validan ownership (se hace en Handler)
- [ ] **Constructor de validators con `?? throw new ArgumentNullException`**

### AutoMapper Profiles
- [ ] `PlantillaProyectoProfile` con mappings calculados (totales, fases, resumen)
- [ ] `RolProfesionalProfile` con mappings basico y con categoria
- [ ] `CategoriaRolProfile` con mapping simple 1:1
- [ ] Profiles separados por entidad (NO un mega-profile)

### Reglas CQRS CRITICAS
- [ ] Handler + Command/Query en MISMO archivo (5 archivos total)
- [ ] SIEMPRE retornar `ServiceResponse<T>`
- [ ] Handler NUNCA inyecta DbContext (solo Services)
- [ ] Services retornan entidades (NO DTOs)
- [ ] **Validators con `.WithMessage()` Y `.WithErrorCode(ServiceResponseMessageType.X)`**
- [ ] **Validators en carpeta `Validators/` separada**
- [ ] **SIEMPRE `?? throw new ArgumentNullException` en constructores**
- [ ] **SIEMPRE usar `ServiceResponseMessageType.X` constants (NO strings literales)**
- [ ] Try-catch con logging en Handlers
- [ ] Caching solo desde Services (ADR-006)

---

## 9. Dependencias de Servicios (Interfaces Requeridas)

Los handlers requieren estos servicios (ya implementados en `hexagonal-architecture.md`):

| Interfaz | Metodos Requeridos | Usado En |
|----------|-------------------|----------|
| `IPlantillaProyectoService` | GetAllActivosAsync(), GetByIdAsync(), GetByIdWithNecesidadesAsync() | Queries, Validator, Handler |
| `IPlantillaProyectoNecesidadService` | GetByIdsAsync(List&lt;Guid&gt;) | Handler (opcional) |
| `IProyectoArtisticoService` | GetByIdAsync() | Validator, Handler (ownership) |
| `INecesidadCrowdsourcingService` | CreateManyAsync(List&lt;NecesidadCrowdsourcing&gt;) | Handler |
| `IRolProfesionalService` | GetAllActivosAsync() | Query |
| `ICategoriaRolService` | GetAllAsync() | Query |
| `IRequestCacheService` | GetOrAddAsync() | Todos los services (para cache) |

**NOTA:** La interfaz `INecesidadCrowdsourcingService` requiere un metodo nuevo:
```csharp
Task<List<Guid>> CreateManyAsync(List<NecesidadCrowdsourcing> entities, CancellationToken ct);
```
Este metodo debe crear multiples necesidades en una transaccion atomica y retornar los IDs generados.

---

## 10. Siguiente Paso

**Este plan es input para implementacion de la capa Application.**

**Orden sugerido de implementacion:**

1. **AutoMapper Profiles** (sin dependencias, solo mappings)
2. **Validators** (dependen de Services)
3. **Queries + Handlers** (lectura, sin cambios en DB)
4. **Command + Handler** (escritura, mas complejo, requiere transaccion atomica)
5. **Testing unitario** de Validators y Handlers

**Prerequisitos antes de implementar:**
- Domain + Infrastructure layer completado (entidades, repositories, services)
- DTOs creados en `api-contracts.md`
- ServiceResponseMessageType constants agregadas (ErrorCodes 2006-2008)
- Seed data aplicado en BD (6 templates, ~55 necesidades, ~35 roles)

**Siguiente plan:**
- Controllers plan (orquestacion de MediatR, autorizacion, Swagger)
- E2E testing plan (wizard completo desde frontend)

---

**Plan creado:** 2026-02-15
**Autor:** Claude Code (CQRS Planning Architect)
**Estado:** READY FOR IMPLEMENTATION
**Archivos totales:** 10 nuevos (5 Commands/Queries, 2 Validators, 3 Profiles)
