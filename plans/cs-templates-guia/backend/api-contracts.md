# Contratos API: Templates y Guia para Artistas Noveles

**Fecha:** 2026-02-15
**Modulo:** Crowdfunding
**Feature:** cs-templates-guia (US-CS-01)
**Basado en:** docs/user-stories/cs-templates-guia/contracts.md

---

## 1. Resumen Ejecutivo

Este plan detalla los contratos API (DTOs, Validadores, Mappings, Swagger) para la feature de templates de crowdsourcing. El sistema permite a artistas seleccionar plantillas pre-configuradas de proyectos musicales y generar necesidades profesionales masivamente.

**Componentes clave:**
- 8 DTOs de respuesta
- 2 DTOs de request (commands)
- 5 Queries (GET endpoints)
- 1 Command (POST endpoint)
- 2 Validators con FluentValidation
- 3 AutoMapper Profiles
- 2 Controllers (CrowdsourcingTemplatesController, MaestrasCrowdsourcingController)
- Constantes de error (2006-2008 para NotFound)

**Arquitectura:** CQRS con MediatR, ServiceResponse wrapper, FluentValidation, AutoMapper.

---

## 2. Endpoints Overview

| Metodo | Ruta | Tipo CQRS | Handler | Descripcion |
|--------|------|-----------|---------|-------------|
| GET | /api/crowdsourcing/templates | Query | GetPlantillasProyectoQuery | Listar plantillas activas |
| GET | /api/crowdsourcing/templates/{id} | Query | GetPlantillaProyectoByIdQuery | Detalle de plantilla |
| POST | /api/crowdsourcing/templates/{id}/generar | Command | GenerarNecesidadesDesdeTemplateCommand | Generar necesidades |
| GET | /api/crowdsourcing/maestras/roles-profesionales | Query | GetRolesProfesionalesQuery | Catalogo de roles |
| GET | /api/crowdsourcing/maestras/categorias-rol | Query | GetCategoriasRolQuery | Categorias de roles |

**Autorizacion:** Todos requieren Bearer JWT. El POST valida ownership del proyecto.

---

## 3. DTOs Response (Lectura)

### 3.1 PlantillaProyectoListDto

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/PlantillaProyectoListDto.cs`

**Proposito:** Resumen de plantilla para galeria (Paso 1 del wizard).

| Propiedad | Tipo | Descripcion | Notas |
|-----------|------|-------------|-------|
| Id | Guid | ID unico | PK |
| Nombre | string | "Produccion de EP" | NOT NULL |
| Descripcion | string? | Texto explicativo | Nullable |
| Icono | string? | Emoji o nombre de icono | Nullable |
| Orden | int | Orden de presentacion | Para ordenar UI |
| PrecioMinTotal | decimal | Suma de precios min orientativos | Calculado |
| PrecioMaxTotal | decimal | Suma de precios max orientativos | Calculado |
| Moneda | int | ID moneda (1 = EUR) | FK MaestraMoneda |
| CantidadNecesidades | int | Cantidad de necesidades | Count |
| Fases | List&lt;string&gt; | Fases unicas (sin duplicados) | Distinct |

**Implementacion:**

```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// Resumen de plantilla de proyecto para galeria (Paso 1 wizard)
/// </summary>
public class PlantillaProyectoListDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; }
    public decimal PrecioMinTotal { get; set; }
    public decimal PrecioMaxTotal { get; set; }
    public int Moneda { get; set; }
    public int CantidadNecesidades { get; set; }
    public List<string> Fases { get; set; } = new();
}
```

### 3.2 PlantillaProyectoDto

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/PlantillaProyectoDto.cs`

**Proposito:** Detalle completo de plantilla con necesidades (Paso 2 del wizard).

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | ID unico |
| Nombre | string | Nombre plantilla |
| Descripcion | string? | Descripcion completa |
| Icono | string? | Icono/emoji |
| Orden | int | Orden de presentacion |
| Necesidades | List&lt;PlantillaProyectoNecesidadDto&gt; | Lista de necesidades |
| Resumen | PlantillaResumenDto | Resumen financiero y prioridades |

**Implementacion:**

```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// Detalle completo de plantilla con necesidades (Paso 2 wizard)
/// </summary>
public class PlantillaProyectoDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; }
    public List<PlantillaProyectoNecesidadDto> Necesidades { get; set; } = new();
    public PlantillaResumenDto Resumen { get; set; } = null!;
}
```

### 3.3 PlantillaProyectoNecesidadDto

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/PlantillaProyectoNecesidadDto.cs`

**Proposito:** Necesidad profesional dentro de una plantilla.

| Propiedad | Tipo | Descripcion | Validacion |
|-----------|------|-------------|------------|
| Id | Guid | ID unico | PK |
| Fase | string | "Preproduccion", "Grabacion", etc. | NOT NULL |
| Titulo | string | "Mezcla de pistas" | NOT NULL, MaxLength(200) |
| Descripcion | string? | Explicacion detallada | Nullable |
| RolProfesional | RolProfesionalDto | Rol profesional anidado | NOT NULL |
| PrecioMinOrientativo | decimal? | Precio min sugerido | Nullable, >= 0 |
| PrecioMaxOrientativo | decimal? | Precio max sugerido | Nullable, >= PrecioMin |
| Moneda | int | ID moneda | FK |
| Prioridad | string | "Alta", "Media", "Baja" | NOT NULL |
| Orden | int | Orden dentro de plantilla | Para ordenar UI |

**Implementacion:**

```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// Necesidad profesional dentro de una plantilla
/// </summary>
public class PlantillaProyectoNecesidadDto
{
    public Guid Id { get; set; }
    public string Fase { get; set; } = null!;
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public RolProfesionalDto RolProfesional { get; set; } = null!;
    public decimal? PrecioMinOrientativo { get; set; }
    public decimal? PrecioMaxOrientativo { get; set; }
    public int Moneda { get; set; }
    public string Prioridad { get; set; } = null!; // "Alta", "Media", "Baja"
    public int Orden { get; set; }
}
```

### 3.4 PlantillaResumenDto

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/PlantillaResumenDto.cs`

**Proposito:** Resumen financiero y de prioridades de una plantilla.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| PrecioMinTotal | decimal | Suma de precios min orientativos |
| PrecioMaxTotal | decimal | Suma de precios max orientativos |
| Moneda | int | ID moneda |
| CantidadNecesidadesAlta | int | Count de necesidades con prioridad "Alta" |
| CantidadNecesidadesMedia | int | Count de necesidades con prioridad "Media" |
| CantidadNecesidadesBaja | int | Count de necesidades con prioridad "Baja" |

**Implementacion:**

```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// Resumen financiero y de prioridades de una plantilla
/// </summary>
public class PlantillaResumenDto
{
    public decimal PrecioMinTotal { get; set; }
    public decimal PrecioMaxTotal { get; set; }
    public int Moneda { get; set; }
    public int CantidadNecesidadesAlta { get; set; }
    public int CantidadNecesidadesMedia { get; set; }
    public int CantidadNecesidadesBaja { get; set; }
}
```

### 3.5 RolProfesionalDto

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/RolProfesionalDto.cs`

**Proposito:** Rol profesional basico (usado dentro de necesidad).

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | int | ID rol (int PK) |
| Nombre | string | "Productor Musical" |
| Descripcion | string? | Tooltip educativo |
| CategoriaRolId | int | FK a categoria |
| ModalidadCobro | string? | "Por proyecto", "Por cancion", etc. |

**Implementacion:**

```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// Rol profesional basico (usado dentro de necesidad)
/// </summary>
public class RolProfesionalDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int CategoriaRolId { get; set; }
    public string? ModalidadCobro { get; set; }
}
```

### 3.6 RolProfesionalConCategoriaDto

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/RolProfesionalConCategoriaDto.cs`

**Proposito:** Rol profesional con categoria anidada (maestras endpoint).

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | int | ID rol |
| Nombre | string | Nombre rol |
| Descripcion | string? | Descripcion detallada |
| CategoriaRol | CategoriaRolDto | Categoria anidada |
| ModalidadCobro | string? | Modalidad de cobro |
| Activo | bool | Si esta activo |

**Implementacion:**

```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// Rol profesional con categoria anidada (maestras endpoint)
/// </summary>
public class RolProfesionalConCategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public CategoriaRolDto CategoriaRol { get; set; } = null!;
    public string? ModalidadCobro { get; set; }
    public bool Activo { get; set; }
}
```

### 3.7 CategoriaRolDto

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CategoriaRolDto.cs`

**Proposito:** Categoria de roles profesionales.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | int | ID categoria |
| Nombre | string | "Produccion Musical" |
| Icono | string? | Icono/emoji |
| Orden | int | Orden de presentacion |

**Implementacion:**

```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// Categoria de roles profesionales
/// </summary>
public class CategoriaRolDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Icono { get; set; }
    public int Orden { get; set; }
}
```

### 3.8 GenerarNecesidadesResultDto

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/GenerarNecesidadesResultDto.cs`

**Proposito:** Resultado de generacion masiva de necesidades desde template.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| NecesidadesCreadas | int | Cantidad creada |
| NecesidadIds | List&lt;Guid&gt; | IDs generados |
| PresupuestoTotalMin | decimal | Suma total min |
| PresupuestoTotalMax | decimal | Suma total max |
| Moneda | int | ID moneda |

**Implementacion:**

```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// Resultado de generacion masiva de necesidades desde template
/// </summary>
public class GenerarNecesidadesResultDto
{
    public int NecesidadesCreadas { get; set; }
    public List<Guid> NecesidadIds { get; set; } = new();
    public decimal PresupuestoTotalMin { get; set; }
    public decimal PresupuestoTotalMax { get; set; }
    public int Moneda { get; set; }
}
```

---

## 4. DTOs Request (Commands/Queries)

### 4.1 GetPlantillasProyectoQuery

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Templates/Queries/GetPlantillasProyectoQuery.cs`

**Proposito:** Listar plantillas activas con resumen.

**Implementa:** `IRequest<ServiceResponse<List<PlantillaProyectoListDto>>>`

**No tiene parametros** - Lista todas las plantillas activas ordenadas por `Orden`.

**Implementacion:**

```csharp
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;

namespace WePlayRises.Crowdfunding.Application.Features.Templates.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetPlantillasProyectoQuery : IRequest<ServiceResponse<List<PlantillaProyectoListDto>>>
{
    // No parameters - returns all active templates
}
```

### 4.2 GetPlantillaProyectoByIdQuery

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Templates/Queries/GetPlantillaProyectoByIdQuery.cs`

**Proposito:** Obtener detalle de plantilla con necesidades.

**Implementa:** `IRequest<ServiceResponse<PlantillaProyectoDto>>`

| Propiedad | Tipo | Validacion |
|-----------|------|------------|
| Id | Guid | NotEmpty |

**Implementacion:**

```csharp
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;

namespace WePlayRises.Crowdfunding.Application.Features.Templates.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetPlantillaProyectoByIdQuery : IRequest<ServiceResponse<PlantillaProyectoDto>>
{
    public Guid Id { get; set; }
}
```

### 4.3 GenerarNecesidadesDesdeTemplateCommand

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Templates/Commands/GenerarNecesidadesDesdeTemplateCommand.cs`

**Proposito:** Generar necesidades masivamente desde template con presupuestos personalizados.

**Implementa:** `IRequest<ServiceResponse<GenerarNecesidadesResultDto>>`

| Propiedad | Tipo | Descripcion | Validacion |
|-----------|------|-------------|------------|
| PlantillaId | Guid | ID de plantilla | NotEmpty, debe existir |
| ProyectoArtisticoId | Guid | ID proyecto | NotEmpty, debe existir y pertenecer al artista |
| NecesidadesSeleccionadas | List&lt;NecesidadSeleccionadaDto&gt; | Items seleccionados | NotEmpty, min 1 item |

**Implementacion:**

```csharp
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;

namespace WePlayRises.Crowdfunding.Application.Features.Templates.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class GenerarNecesidadesDesdeTemplateCommand : IRequest<ServiceResponse<GenerarNecesidadesResultDto>>
{
    public Guid PlantillaId { get; set; }
    public Guid ProyectoArtisticoId { get; set; }
    public List<NecesidadSeleccionadaDto> NecesidadesSeleccionadas { get; set; } = new();
}

/// <summary>
/// Item de necesidad seleccionada con presupuesto personalizado
/// </summary>
public class NecesidadSeleccionadaDto
{
    public Guid PlantillaNecesidadId { get; set; }
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int MonedaId { get; set; }
}
```

### 4.4 GetRolesProfesionalesQuery

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Maestras/Queries/GetRolesProfesionalesQuery.cs`

**Proposito:** Obtener catalogo de roles profesionales con categoria.

**Implementa:** `IRequest<ServiceResponse<List<RolProfesionalConCategoriaDto>>>`

**No parametros** - Retorna todos los roles activos.

**Implementacion:**

```csharp
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;

namespace WePlayRises.Crowdfunding.Application.Features.Maestras.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetRolesProfesionalesQuery : IRequest<ServiceResponse<List<RolProfesionalConCategoriaDto>>>
{
    // No parameters - returns all active roles
}
```

### 4.5 GetCategoriasRolQuery

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Maestras/Queries/GetCategoriasRolQuery.cs`

**Proposito:** Obtener categorias de roles.

**Implementa:** `IRequest<ServiceResponse<List<CategoriaRolDto>>>`

**No parametros** - Retorna todas las categorias ordenadas.

**Implementacion:**

```csharp
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;

namespace WePlayRises.Crowdfunding.Application.Features.Maestras.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetCategoriasRolQuery : IRequest<ServiceResponse<List<CategoriaRolDto>>>
{
    // No parameters - returns all categories
}
```

---

## 5. Validators (FluentValidation)

### 5.1 GenerarNecesidadesDesdeTemplateValidator

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Templates/Validators/GenerarNecesidadesDesdeTemplateValidator.cs`

**Reglas:**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| PlantillaId | NotEmpty | "El ID de la plantilla es obligatorio" | Validation_Required |
| ProyectoArtisticoId | NotEmpty | "El ID del proyecto artistico es obligatorio" | Validation_Required |
| NecesidadesSeleccionadas | NotEmpty | "Debe seleccionar al menos una necesidad" | Validation_Required |
| NecesidadesSeleccionadas | Must(x => x.Count > 0) | "Debe seleccionar al menos una necesidad" | Validation_Required |
| NecesidadesSeleccionadas[].PlantillaNecesidadId | NotEmpty | "El ID de la necesidad es obligatorio" | Validation_Required |
| NecesidadesSeleccionadas[].PresupuestoMin | GreaterThanOrEqualTo(0) when present | "El presupuesto minimo no puede ser negativo" | Validation_InvalidRange |
| NecesidadesSeleccionadas[].PresupuestoMax | GreaterThanOrEqualTo(PresupuestoMin) when both present | "El presupuesto maximo debe ser mayor o igual al minimo" | Validation_InvalidRange |
| NecesidadesSeleccionadas[].MonedaId | GreaterThan(0) | "La moneda es obligatoria" | Validation_Required |

**Validaciones de negocio (async):**

- Plantilla existe y esta activa (via IPlantillaProyectoService con cache)
- ProyectoArtistico existe (via IProyectoArtisticoService con cache)
- ProyectoArtistico pertenece al artista autenticado (comparar UserId del comando con entidad)
- Todos los PlantillaNecesidadId existen en la plantilla seleccionada

**Implementacion:**

```csharp
using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Templates.Commands;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Templates.Validators;

/// <summary>
/// Validador para GenerarNecesidadesDesdeTemplateCommand
/// Valida presupuestos, plantilla, proyecto y ownership
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

        // Validaciones sincronas basicas
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

        // Validaciones de cada item de necesidad
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

        // Validaciones asincronas de negocio
        RuleFor(x => x.PlantillaId)
            .MustAsync(PlantillaExisteYEstaActiva)
            .WithMessage("La plantilla no existe o no esta activa")
            .WithErrorCode(ServiceResponseMessageType.NotFound_PlantillaProyecto);

        RuleFor(x => x.ProyectoArtisticoId)
            .MustAsync(ProyectoExiste)
            .WithMessage("El proyecto artistico no existe")
            .WithErrorCode(ServiceResponseMessageType.NotFound_ProyectoArtistico);

        // NOTA: La validacion de ownership se hace en el handler porque requiere UserId del claim
        // No se puede hacer aqui porque el validator no tiene acceso al HttpContext
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

### 5.2 GetPlantillaProyectoByIdValidator

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Templates/Validators/GetPlantillaProyectoByIdValidator.cs`

**Reglas:**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| Id | NotEmpty | "El ID de la plantilla es obligatorio" | Validation_Required |

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

## 6. AutoMapper Profiles

### 6.1 PlantillaProyectoProfile

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/PlantillaProyectoProfile.cs`

| Source | Destination | Notas |
|--------|-------------|-------|
| PlantillaProyecto (entity) | PlantillaProyectoListDto | Con calculos de totales y fases |
| PlantillaProyecto (entity) | PlantillaProyectoDto | Con necesidades y resumen anidados |
| PlantillaProyectoNecesidad (entity) | PlantillaProyectoNecesidadDto | Con RolProfesional anidado |

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
        // Entity -> DTO List (para galeria)
        CreateMap<PlantillaProyecto, PlantillaProyectoListDto>()
            .ForMember(dest => dest.PrecioMinTotal,
                opt => opt.MapFrom(src => src.Necesidades.Sum(n => n.PrecioMinOrientativo ?? 0)))
            .ForMember(dest => dest.PrecioMaxTotal,
                opt => opt.MapFrom(src => src.Necesidades.Sum(n => n.PrecioMaxOrientativo ?? 0)))
            .ForMember(dest => dest.CantidadNecesidades,
                opt => opt.MapFrom(src => src.Necesidades.Count))
            .ForMember(dest => dest.Fases,
                opt => opt.MapFrom(src => src.Necesidades
                    .Select(n => n.Fase)
                    .Distinct()
                    .OrderBy(f => f)
                    .ToList()));

        // Entity -> DTO Detail (con necesidades)
        CreateMap<PlantillaProyecto, PlantillaProyectoDto>()
            .ForMember(dest => dest.Necesidades,
                opt => opt.MapFrom(src => src.Necesidades.OrderBy(n => n.Orden)))
            .ForMember(dest => dest.Resumen,
                opt => opt.MapFrom(src => new PlantillaResumenDto
                {
                    PrecioMinTotal = src.Necesidades.Sum(n => n.PrecioMinOrientativo ?? 0),
                    PrecioMaxTotal = src.Necesidades.Sum(n => n.PrecioMaxOrientativo ?? 0),
                    Moneda = src.Necesidades.FirstOrDefault()?.MonedaId ?? 1,
                    CantidadNecesidadesAlta = src.Necesidades.Count(n => n.Prioridad == "Alta"),
                    CantidadNecesidadesMedia = src.Necesidades.Count(n => n.Prioridad == "Media"),
                    CantidadNecesidadesBaja = src.Necesidades.Count(n => n.Prioridad == "Baja")
                }));

        // PlantillaProyectoNecesidad -> DTO
        CreateMap<PlantillaProyectoNecesidad, PlantillaProyectoNecesidadDto>()
            .ForMember(dest => dest.RolProfesional,
                opt => opt.MapFrom(src => src.RolProfesional));
    }
}
```

### 6.2 RolProfesionalProfile

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/RolProfesionalProfile.cs`

| Source | Destination | Notas |
|--------|-------------|-------|
| MaestraRolProfesional (entity) | RolProfesionalDto | Basico sin categoria |
| MaestraRolProfesional (entity) | RolProfesionalConCategoriaDto | Con categoria anidada |

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

### 6.3 CategoriaRolProfile

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/CategoriaRolProfile.cs`

| Source | Destination | Notas |
|--------|-------------|-------|
| MaestraCategoriaRol (entity) | CategoriaRolDto | Simple 1:1 |

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

## 7. Controllers

### 7.1 CrowdsourcingTemplatesController

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/CrowdsourcingTemplatesController.cs`

**Ruta base:** `/api/crowdsourcing/templates`

**Endpoints:**

| Metodo | Ruta | Action | MediatR Query/Command |
|--------|------|--------|----------------------|
| GET | / | GetTemplates | GetPlantillasProyectoQuery |
| GET | /{id} | GetTemplateById | GetPlantillaProyectoByIdQuery |
| POST | /{id}/generar | GenerarNecesidades | GenerarNecesidadesDesdeTemplateCommand |

**Implementacion:**

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WePlayRises.Crowdfunding.Application.Features.Templates.Commands;
using WePlayRises.Crowdfunding.Application.Features.Templates.Queries;

namespace WePlayRises.Crowdfunding.WebApi.Controllers;

/// <summary>
/// Endpoints para plantillas de proyectos de crowdsourcing
/// </summary>
[ApiController]
[Route("api/crowdsourcing/templates")]
[Authorize] // Todos los endpoints requieren autenticacion
public class CrowdsourcingTemplatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CrowdsourcingTemplatesController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Lista las plantillas de proyecto activas disponibles para artistas
    /// </summary>
    /// <returns>Lista de plantillas con resumen (precio, cantidad necesidades, fases)</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ServiceResponse<List<PlantillaProyectoListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTemplates()
    {
        var query = new GetPlantillasProyectoQuery();
        var response = await _mediator.Send(query);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Obtiene el detalle completo de una plantilla con todas sus necesidades
    /// </summary>
    /// <param name="id">ID de la plantilla</param>
    /// <returns>Detalle de plantilla con necesidades y resumen</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ServiceResponse<PlantillaProyectoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTemplateById([FromRoute] Guid id)
    {
        var query = new GetPlantillaProyectoByIdQuery { Id = id };
        var response = await _mediator.Send(query);

        if (!response.IsSuccess)
        {
            if (response.Messages.Any(m => m.ErrorCode == "2006")) // NotFound_PlantillaProyecto
                return NotFound(response);

            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Genera necesidades de crowdsourcing a partir de una plantilla seleccionada
    /// </summary>
    /// <param name="id">ID de la plantilla</param>
    /// <param name="request">Proyecto artistico y necesidades seleccionadas con presupuestos</param>
    /// <returns>Resumen de necesidades creadas con IDs y totales</returns>
    [HttpPost("{id}/generar")]
    [ProducesResponseType(typeof(ServiceResponse<GenerarNecesidadesResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerarNecesidades(
        [FromRoute] Guid id,
        [FromBody] GenerarNecesidadesRequest request)
    {
        var command = new GenerarNecesidadesDesdeTemplateCommand
        {
            PlantillaId = id,
            ProyectoArtisticoId = request.ProyectoArtisticoId,
            NecesidadesSeleccionadas = request.NecesidadesSeleccionadas
        };

        var response = await _mediator.Send(command);

        if (!response.IsSuccess)
        {
            if (response.Messages.Any(m => m.ErrorCode.StartsWith("20"))) // NotFound errors
                return NotFound(response);

            if (response.Messages.Any(m => m.ErrorCode == "3002")) // Forbidden
                return Forbid();

            return BadRequest(response);
        }

        return Created($"/api/crowdsourcing/proyectos/{request.ProyectoArtisticoId}/necesidades", response);
    }
}

/// <summary>
/// Request DTO for GenerarNecesidades endpoint (binded from body)
/// </summary>
public class GenerarNecesidadesRequest
{
    public Guid ProyectoArtisticoId { get; set; }
    public List<NecesidadSeleccionadaDto> NecesidadesSeleccionadas { get; set; } = new();
}
```

### 7.2 MaestrasCrowdsourcingController

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/MaestrasCrowdsourcingController.cs`

**Ruta base:** `/api/crowdsourcing/maestras`

**Endpoints:**

| Metodo | Ruta | Action | MediatR Query |
|--------|------|--------|--------------|
| GET | /roles-profesionales | GetRolesProfesionales | GetRolesProfesionalesQuery |
| GET | /categorias-rol | GetCategoriasRol | GetCategoriasRolQuery |

**Implementacion:**

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WePlayRises.Crowdfunding.Application.Features.Maestras.Queries;

namespace WePlayRises.Crowdfunding.WebApi.Controllers;

/// <summary>
/// Endpoints para maestras (catalogos) de crowdsourcing
/// </summary>
[ApiController]
[Route("api/crowdsourcing/maestras")]
[Authorize] // Usuarios autenticados
public class MaestrasCrowdsourcingController : ControllerBase
{
    private readonly IMediator _mediator;

    public MaestrasCrowdsourcingController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Obtiene el catalogo completo de roles profesionales con categoria
    /// </summary>
    /// <returns>Lista de roles profesionales activos</returns>
    [HttpGet("roles-profesionales")]
    [ProducesResponseType(typeof(ServiceResponse<List<RolProfesionalConCategoriaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRolesProfesionales()
    {
        var query = new GetRolesProfesionalesQuery();
        var response = await _mediator.Send(query);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Obtiene las categorias de roles profesionales
    /// </summary>
    /// <returns>Lista de categorias ordenadas</returns>
    [HttpGet("categorias-rol")]
    [ProducesResponseType(typeof(ServiceResponse<List<CategoriaRolDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCategoriasRol()
    {
        var query = new GetCategoriasRolQuery();
        var response = await _mediator.Send(query);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }
}
```

---

## 8. ServiceResponseMessageType Constants (Additions)

**Ubicacion:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Constants/ServiceResponseMessageType.cs`

**Agregar estos nuevos codigos:**

```csharp
// =========================================================================
// NOT FOUND ERRORS (2000-2999) - ADDITIONS
// =========================================================================
public const string NotFound_PlantillaProyecto = "2006";
public const string NotFound_ProyectoArtistico = "2007";
public const string NotFound_PlantillaNecesidad = "2008";
```

**Codigo completo con adiciones:**

```csharp
namespace WePlayRises.Crowdfunding.Domain.Constants;

/// <summary>
/// Constantes para codigos de error en ServiceResponse.
/// Usar SIEMPRE estas constantes en .WithErrorCode() de FluentValidation y en Handlers.
/// </summary>
public static class ServiceResponseMessageType
{
    // =========================================================================
    // SUCCESS (0000-0999)
    // =========================================================================
    public const string Success = "0000";
    public const string Created = "0001";
    public const string Updated = "0002";
    public const string Deleted = "0003";

    // =========================================================================
    // VALIDATION ERRORS (1000-1999)
    // =========================================================================
    public const string Validation_Required = "1001";
    public const string Validation_MaxLength = "1002";
    public const string Validation_MinLength = "1003";
    public const string Validation_InvalidFormat = "1004";
    public const string Validation_InvalidEmail = "1005";
    public const string Validation_InvalidUrl = "1006";
    public const string Validation_InvalidRange = "1007";
    public const string Validation_DuplicateName = "1008";
    public const string Validation_DuplicateEmail = "1009";
    public const string Validation_ForeignKeyNotFound = "1010";
    public const string Validation_InvalidAmount = "1011";
    public const string Validation_InvalidDate = "1012";

    // =========================================================================
    // NOT FOUND ERRORS (2000-2999)
    // =========================================================================
    public const string NotFound_Entity = "2000";
    public const string NotFound_Campania = "2003";
    public const string NotFound_Reward = "2004";
    public const string NotFound_Backing = "2005";
    public const string NotFound_Artista = "2002";

    // NEW - Crowdsourcing Templates (2006-2008)
    public const string NotFound_PlantillaProyecto = "2006";
    public const string NotFound_ProyectoArtistico = "2007";
    public const string NotFound_PlantillaNecesidad = "2008";

    // =========================================================================
    // AUTHENTICATION/AUTHORIZATION ERRORS (3000-3999)
    // =========================================================================
    public const string Auth_Unauthorized = "3001";
    public const string Auth_Forbidden = "3002";
    public const string Auth_TokenExpired = "3003";
    public const string Auth_InvalidToken = "3004";
    public const string Auth_UserNotAuthenticated = "3005";

    // =========================================================================
    // BUSINESS RULE ERRORS (4000-4999)
    // =========================================================================
    public const string BusinessRule_DuplicateRecord = "4001";
    public const string BusinessRule_InvalidState = "4002";
    public const string BusinessRule_OperationNotAllowed = "4003";
    public const string BusinessRule_LimitExceeded = "4004";
    public const string BusinessRule_InsufficientFunds = "4005";
    public const string BusinessRule_CampaniaNotActive = "4006";
    public const string BusinessRule_CampaniaEnded = "4007";
    public const string BusinessRule_CampaniaNotDraft = "4009";
    public const string BusinessRule_RewardHasBackings = "4010";
    public const string BusinessRule_RewardOutOfStock = "4011";
    public const string BusinessRule_AmountBelowMinimum = "4012";
    public const string BusinessRule_AnonymousNotAllowed = "4013";

    // =========================================================================
    // INTERNAL ERRORS (5000-5999)
    // =========================================================================
    public const string Internal_UnexpectedError = "5000";
    public const string Internal_DatabaseError = "5001";
    public const string Internal_ExternalServiceError = "5002";
    public const string Internal_ConfigurationError = "5003";
}
```

---

## 9. OpenAPI / Swagger Documentation

### 9.1 Annotations por Endpoint

**GET /api/crowdsourcing/templates**

- **Summary:** Lista plantillas de proyecto activas
- **Description:** Retorna todas las plantillas disponibles para artistas con resumen de precio, cantidad de necesidades y fases
- **Request Body:** N/A (GET)
- **Responses:**
  - 200: ServiceResponse&lt;List&lt;PlantillaProyectoListDto&gt;&gt;
  - 401: No autenticado (Bearer token invalido/expirado)
  - 500: Error inesperado
- **Auth:** Bearer JWT requerido (Artista o usuario autenticado)
- **Tags:** Crowdsourcing Templates

**GET /api/crowdsourcing/templates/{id}**

- **Summary:** Obtiene detalle de plantilla
- **Description:** Retorna detalle completo de una plantilla con todas sus necesidades organizadas por fase
- **Parameters:**
  - id (path, Guid, required) - ID de la plantilla
- **Responses:**
  - 200: ServiceResponse&lt;PlantillaProyectoDto&gt;
  - 400: Validacion fallida (ID invalido)
  - 404: Plantilla no encontrada (ErrorCode 2006)
  - 401: No autenticado
  - 500: Error inesperado
- **Auth:** Bearer JWT requerido
- **Tags:** Crowdsourcing Templates

**POST /api/crowdsourcing/templates/{id}/generar**

- **Summary:** Genera necesidades desde plantilla
- **Description:** Crea multiples NecesidadCrowdsourcing en una transaccion atomica a partir de una plantilla seleccionada con presupuestos personalizados
- **Parameters:**
  - id (path, Guid, required) - ID de la plantilla
- **Request Body:** GenerarNecesidadesRequest (application/json)
  ```json
  {
    "proyectoArtisticoId": "guid",
    "necesidadesSeleccionadas": [
      {
        "plantillaNecesidadId": "guid",
        "presupuestoMin": 100.00,
        "presupuestoMax": 500.00,
        "monedaId": 1
      }
    ]
  }
  ```
- **Responses:**
  - 201: ServiceResponse&lt;GenerarNecesidadesResultDto&gt; - Necesidades creadas exitosamente
  - 400: Validacion fallida (presupuestos invalidos, sin necesidades seleccionadas)
  - 403: Forbidden (ErrorCode 3002) - El proyecto no pertenece al artista autenticado
  - 404: Not Found (ErrorCodes 2006, 2007, 2008) - Plantilla, Proyecto o Necesidad no encontrada
  - 401: No autenticado
  - 500: Error inesperado
- **Auth:** Bearer JWT requerido (valida ownership del proyecto)
- **Tags:** Crowdsourcing Templates

**GET /api/crowdsourcing/maestras/roles-profesionales**

- **Summary:** Catalogo de roles profesionales
- **Description:** Retorna todos los roles profesionales activos con su categoria, descripcion y modalidad de cobro
- **Request Body:** N/A
- **Responses:**
  - 200: ServiceResponse&lt;List&lt;RolProfesionalConCategoriaDto&gt;&gt;
  - 401: No autenticado
  - 500: Error inesperado
- **Auth:** Bearer JWT requerido
- **Tags:** Maestras Crowdsourcing

**GET /api/crowdsourcing/maestras/categorias-rol**

- **Summary:** Categorias de roles
- **Description:** Retorna las 6 categorias de roles profesionales (Produccion Musical, Audiovisual, etc.)
- **Request Body:** N/A
- **Responses:**
  - 200: ServiceResponse&lt;List&lt;CategoriaRolDto&gt;&gt;
  - 401: No autenticado
  - 500: Error inesperado
- **Auth:** Bearer JWT requerido
- **Tags:** Maestras Crowdsourcing

### 9.2 XML Comments (Ejemplo para Swagger UI)

Los controllers ya incluyen XML comments (`/// <summary>`) en la implementacion del apartado 7. Estos se reflejaran automaticamente en Swagger UI si el proyecto tiene configurado:

```xml
<GenerateDocumentationFile>true</GenerateDocumentationFile>
```

---

## 10. Archivos a Crear

```
Modules/Crowdfunding/
├── WePlayRises.Crowdfunding.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs           (MODIFICAR - agregar 2006-2008)
│
├── WePlayRises.Crowdfunding.Application/
│   ├── Dtos/
│   │   ├── PlantillaProyectoListDto.cs            (NUEVO)
│   │   ├── PlantillaProyectoDto.cs                (NUEVO)
│   │   ├── PlantillaProyectoNecesidadDto.cs       (NUEVO)
│   │   ├── PlantillaResumenDto.cs                 (NUEVO)
│   │   ├── RolProfesionalDto.cs                   (NUEVO)
│   │   ├── RolProfesionalConCategoriaDto.cs       (NUEVO)
│   │   ├── CategoriaRolDto.cs                     (NUEVO)
│   │   └── GenerarNecesidadesResultDto.cs         (NUEVO)
│   │
│   ├── Features/
│   │   ├── Templates/
│   │   │   ├── Queries/
│   │   │   │   ├── GetPlantillasProyectoQuery.cs          (NUEVO - Query + Handler)
│   │   │   │   └── GetPlantillaProyectoByIdQuery.cs       (NUEVO - Query + Handler)
│   │   │   ├── Commands/
│   │   │   │   └── GenerarNecesidadesDesdeTemplateCommand.cs (NUEVO - Command + Handler)
│   │   │   └── Validators/
│   │   │       ├── GenerarNecesidadesDesdeTemplateValidator.cs (NUEVO)
│   │   │       └── GetPlantillaProyectoByIdValidator.cs        (NUEVO)
│   │   │
│   │   └── Maestras/
│   │       └── Queries/
│   │           ├── GetRolesProfesionalesQuery.cs   (NUEVO - Query + Handler)
│   │           └── GetCategoriasRolQuery.cs        (NUEVO - Query + Handler)
│   │
│   └── Mapping/
│       ├── PlantillaProyectoProfile.cs             (NUEVO)
│       ├── RolProfesionalProfile.cs                (NUEVO)
│       └── CategoriaRolProfile.cs                  (NUEVO)
│
└── WePlayRises.Crowdfunding.WebApi/
    └── Controllers/
        ├── CrowdsourcingTemplatesController.cs     (NUEVO)
        └── MaestrasCrowdsourcingController.cs      (NUEVO)
```

**Total:**
- 8 DTOs nuevos
- 5 Queries (con handlers en mismo archivo)
- 1 Command (con handler en mismo archivo)
- 2 Validators
- 3 AutoMapper Profiles
- 2 Controllers
- 1 Archivo modificado (ServiceResponseMessageType.cs)

---

## 11. Reglas CQRS - Checklist

Validar que TODOS los archivos cumplan:

- [x] **Handler + Command/Query en MISMO archivo** - Todos los Query/Command tienen Handler en el mismo .cs
- [x] **SIEMPRE retornar ServiceResponse&lt;T&gt;** - Todos implementan IRequest&lt;ServiceResponse&lt;T&gt;&gt;
- [x] **Constructor con ?? throw** - Todos los handlers validan dependencias con ?? throw new ArgumentNullException
- [x] **Validators usan ServiceResponseMessageType constants** - No strings literales en .WithErrorCode()
- [x] **Handlers usan ServiceResponseMessageType constants** - Mensajes de exito/error con constantes
- [x] **Logger inyectado** - Todos los handlers tienen ILogger&lt;THandler&gt;
- [x] **Services inyectados (no DbContext)** - Handlers inyectan IService, no DbContext
- [x] **Validacion retorna ServiceResponse** - Validators no hacen throw, retornan ServiceResponse con errores
- [x] **Try-Catch en handlers** - Todos los handlers tienen try-catch con logging
- [x] **AutoMapper Profile separado por entidad** - PlantillaProyectoProfile, RolProfesionalProfile, CategoriaRolProfile
- [x] **Services retornan entidades** - Los services devuelven entidades, handlers hacen mapping a DTO

---

## 12. Dependencias de Servicios (Interfaces)

Los handlers requieren estos servicios (a implementar en el plan de backend):

| Interfaz | Metodos Requeridos | Usado En |
|----------|-------------------|----------|
| IPlantillaProyectoService | GetAllActivasAsync(), GetByIdAsync() | Queries, Validator |
| IPlantillaProyectoNecesidadService | GetByIdsAsync(List&lt;Guid&gt;) | Command |
| IProyectoArtisticoService | GetByIdAsync() | Validator |
| INecesidadCrowdsourcingService | CreateManyAsync(List&lt;NecesidadCrowdsourcing&gt;) | Command |
| IRolProfesionalService | GetAllActivosAsync() | Maestras Query |
| ICategoriaRolService | GetAllAsync() | Maestras Query |
| IRequestCacheService | GetOrAddAsync() | Services (para cache) |

**NOTA:** Estas interfaces se definiran en el plan de backend (`backend-plan.md`).

---

## 13. Ejemplo de Respuesta JSON Completa

### GET /api/crowdsourcing/templates (200 OK)

```json
{
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "nombre": "Producción de EP",
      "descripcion": "Plantilla completa para producir un EP de 4-6 canciones con calidad profesional",
      "icono": "🎵",
      "orden": 1,
      "precioMinTotal": 3000.00,
      "precioMaxTotal": 8000.00,
      "moneda": 1,
      "cantidadNecesidades": 8,
      "fases": ["Preproducción", "Grabación", "Mezcla y Master", "Promoción"]
    }
  ],
  "messages": [
    {
      "message": "Plantillas obtenidas exitosamente",
      "errorCode": "0000"
    }
  ]
}
```

### POST /api/crowdsourcing/templates/{id}/generar (201 Created)

```json
{
  "data": {
    "necesidadesCreadas": 5,
    "necesidadIds": [
      "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
      "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e",
      "c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f"
    ],
    "presupuestoTotalMin": 3300.00,
    "presupuestoTotalMax": 5800.00,
    "moneda": 1
  },
  "messages": [
    {
      "message": "Necesidades generadas exitosamente a partir de la plantilla",
      "errorCode": "0001"
    }
  ]
}
```

### POST /api/crowdsourcing/templates/{id}/generar (400 Bad Request - Validacion)

```json
{
  "data": null,
  "messages": [
    {
      "message": "Debe seleccionar al menos una necesidad",
      "errorCode": "1001"
    },
    {
      "message": "El presupuesto máximo debe ser mayor o igual al mínimo",
      "errorCode": "1007"
    }
  ]
}
```

### POST /api/crowdsourcing/templates/{id}/generar (404 Not Found)

```json
{
  "data": null,
  "messages": [
    {
      "message": "Plantilla no encontrada",
      "errorCode": "2006"
    }
  ]
}
```

### POST /api/crowdsourcing/templates/{id}/generar (403 Forbidden)

```json
{
  "data": null,
  "messages": [
    {
      "message": "No tienes permiso para modificar este proyecto",
      "errorCode": "3002"
    }
  ]
}
```

---

## 14. Notas de Implementacion

### 14.1 Validacion de Ownership

**Importante:** La validacion de que el ProyectoArtistico pertenece al artista autenticado NO se hace en el Validator (no tiene acceso a HttpContext), sino en el **Handler**:

```csharp
// En GenerarNecesidadesDesdeTemplateCommandHandler.Handle()

// Obtener UserId del claim JWT
var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userIdClaim))
{
    return new ServiceResponse<GenerarNecesidadesResultDto>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "Usuario no autenticado", ErrorCode = ServiceResponseMessageType.Auth_Unauthorized }
        }
    };
}

// Validar ownership via service
var proyecto = await _proyectoService.GetByIdAsync(request.ProyectoArtisticoId, ct);
if (proyecto.ArtistaId.ToString() != userIdClaim)
{
    return new ServiceResponse<GenerarNecesidadesResultDto>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "No tienes permiso para modificar este proyecto", ErrorCode = ServiceResponseMessageType.Auth_Forbidden }
        }
    };
}
```

### 14.2 Calculo de Resumen en PlantillaProyectoProfile

El `PlantillaResumenDto` se calcula en AutoMapper usando proyecciones LINQ sobre la coleccion `Necesidades`:

- **PrecioMinTotal:** `src.Necesidades.Sum(n => n.PrecioMinOrientativo ?? 0)`
- **PrecioMaxTotal:** `src.Necesidades.Sum(n => n.PrecioMaxOrientativo ?? 0)`
- **CantidadNecesidadesAlta:** `src.Necesidades.Count(n => n.Prioridad == "Alta")`

**Alternativa:** Calcular en el Handler si AutoMapper no soporta LINQ complejos.

### 14.3 Transaccion Atomica en Command

El `GenerarNecesidadesDesdeTemplateCommand` debe crear N registros de `NecesidadCrowdsourcing` en una sola transaccion. Si usa EF Core:

```csharp
// En el Service INecesidadCrowdsourcingService.CreateManyAsync()
using var transaction = await _context.Database.BeginTransactionAsync(ct);
try
{
    await _context.NecesidadesCrowdsourcing.AddRangeAsync(necesidades, ct);
    await _context.SaveChangesAsync(ct);
    await transaction.CommitAsync(ct);
}
catch
{
    await transaction.RollbackAsync(ct);
    throw;
}
```

### 14.4 Cache de Maestras

Los endpoints de maestras (roles, categorias) retornan datos que cambian poco. Configurar cache en el frontend con `staleTime` largo (5-10 minutos).

Backend puede usar cache en el service:

```csharp
// En RolProfesionalService.GetAllActivosAsync()
return await _requestCache.GetOrAddAsync(
    "roles:all:activos",
    async () => await _repository.GetAllActivosAsync(ct),
    TimeSpan.FromMinutes(10)
);
```

---

## 15. Siguiente Paso

**Este plan es input para:**

1. **Backend Team** - Implementar Handlers, Services, Repositories, Entidades
2. **Frontend Team** - Consumir estos DTOs desde TypeScript (ya planeados en `shared/contracts-plan.md`)

**Orden sugerido de implementacion:**

1. Crear DTOs (archivos simples, sin logica)
2. Crear Validators (dependencias minimas)
3. Crear AutoMapper Profiles (dependencias de entidades)
4. Crear Queries + Handlers (lectura, sin cambios en DB)
5. Crear Command + Handler (escritura, mas complejo)
6. Crear Controllers (orquestacion de MediatR)
7. Agregar constantes de error a ServiceResponseMessageType
8. Testing unitario de Validators y Handlers

---

**Plan creado:** 2026-02-15
**Autor:** Claude Code (Arquitecto Backend)
**Estado:** READY FOR IMPLEMENTATION
**Archivos totales:** 23 nuevos + 1 modificado
