# Contratos API: Gestionar Necesidades de Crowdsourcing

**Fecha:** 2026-02-16
**Modulo:** Crowdsourcing
**Feature:** cs-gestionar-necesidades

---

## 1. Endpoints

| Metodo | Ruta | Tipo | Descripcion |
|--------|------|------|-------------|
| POST | /api/crowdsourcing/necesidades | Command | Crear necesidad |
| GET | /api/crowdsourcing/necesidades/mis-necesidades | Query | Listar mis necesidades (paginado) |
| GET | /api/crowdsourcing/necesidades/{id} | Query | Obtener detalle por ID |
| PUT | /api/crowdsourcing/necesidades/{id} | Command | Actualizar necesidad (solo si Abierta) |
| PATCH | /api/crowdsourcing/necesidades/{id}/cerrar | Command | Cerrar necesidad |

---

## 2. Request DTOs (Controller recibe)

### 2.1 CreateNecesidadRequest

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/CreateNecesidadRequest.cs`

```csharp
namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class CreateNecesidadRequest
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
}
```

| Propiedad | Tipo | Requerido | Validacion |
|-----------|------|-----------|------------|
| Titulo | string | Si | NotEmpty, MinLength(5), MaxLength(200) |
| Descripcion | string? | No | MaxLength(4000) |
| TipoNecesidadId | int | Si | NotEmpty, Exists in maestra |
| ModalidadTrabajoId | int | Si | NotEmpty, Exists in maestra |
| PresupuestoMin | decimal? | No | GreaterThanOrEqualTo(0) |
| PresupuestoMax | decimal? | No | GreaterThanOrEqualTo(PresupuestoMin) |
| MonedaId | int? | Condicional | Required if presupuesto presente |
| UbicacionCiudad | string? | Condicional | Required if modalidad Presencial/Hibrido, MaxLength(100) |
| UbicacionPais | string? | Condicional | Required if modalidad Presencial/Hibrido, MaxLength(100) |
| FechaLimitePropuestas | DateTime? | No | GreaterThan(Today) |
| FechaInicioPrevista | DateTime? | No | GreaterThanOrEqualTo(Today) |
| ProyectoArtisticoId | Guid | Si | NotEmpty, Exists, Belongs to Artista autenticado |

### 2.2 UpdateNecesidadRequest

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/UpdateNecesidadRequest.cs`

```csharp
namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class UpdateNecesidadRequest
{
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
}
```

**Nota:** TipoNecesidadId y ProyectoArtisticoId NO son editables (inmutables una vez creados)

| Propiedad | Tipo | Requerido | Validacion |
|-----------|------|-----------|------------|
| Titulo | string | Si | NotEmpty, MinLength(5), MaxLength(200) |
| Descripcion | string? | No | MaxLength(4000) |
| ModalidadTrabajoId | int | Si | NotEmpty, Exists in maestra |
| PresupuestoMin | decimal? | No | GreaterThanOrEqualTo(0) |
| PresupuestoMax | decimal? | No | GreaterThanOrEqualTo(PresupuestoMin) |
| MonedaId | int? | Condicional | Required if presupuesto presente |
| UbicacionCiudad | string? | Condicional | Required if modalidad Presencial/Hibrido, MaxLength(100) |
| UbicacionPais | string? | Condicional | Required if modalidad Presencial/Hibrido, MaxLength(100) |
| FechaLimitePropuestas | DateTime? | No | GreaterThan(Today) |
| FechaInicioPrevista | DateTime? | No | GreaterThanOrEqualTo(Today) |

### 2.3 CerrarNecesidadRequest

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/CerrarNecesidadRequest.cs`

```csharp
namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class CerrarNecesidadRequest
{
    public string? Motivo { get; set; }
}
```

| Propiedad | Tipo | Requerido | Validacion |
|-----------|------|-----------|------------|
| Motivo | string? | No | MaxLength(500) |

---

## 3. Response DTOs

### 3.1 NecesidadCrowdsourcingListDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/NecesidadCrowdsourcingListDto.cs`

```csharp
namespace WePlayRises.Crowdsourcing.Application.Dtos;

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

**Uso:** Listado paginado en `GET /mis-necesidades`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador unico |
| Titulo | string | Titulo de la necesidad |
| EstadoNecesidadId | int | ID estado (1=Abierta, 2=EnProgreso, 3=Cerrada, 4=Cancelada) |
| EstadoNecesidadNombre | string | Nombre del estado |
| TipoNecesidadId | int | ID tipo de necesidad |
| TipoNecesidadNombre | string | Nombre del tipo |
| PresupuestoMin | decimal? | Presupuesto minimo |
| PresupuestoMax | decimal? | Presupuesto maximo |
| MonedaId | int? | ID moneda |
| MonedaNombre | string? | Codigo moneda (EUR, USD, GBP) |
| ModalidadTrabajoId | int | ID modalidad (1=Presencial, 2=Remoto, 3=Hibrido) |
| ModalidadTrabajoNombre | string | Nombre modalidad |
| NumeroPropuestas | int | Contador de propuestas recibidas |
| FechaCreacion | DateTime | Fecha de publicacion |
| FechaLimitePropuestas | DateTime? | Fecha limite para recibir propuestas |
| FechaActualizacion | DateTime? | Fecha ultima actualizacion |

### 3.2 NecesidadCrowdsourcingDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/NecesidadCrowdsourcingDto.cs`

```csharp
namespace WePlayRises.Crowdsourcing.Application.Dtos;

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

**Uso:** Detalle completo en `GET /necesidades/{id}`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador unico |
| Titulo | string | Titulo de la necesidad |
| Descripcion | string? | Descripcion detallada |
| TipoNecesidadId | int | ID tipo de necesidad |
| TipoNecesidadNombre | string | Nombre del tipo |
| EstadoNecesidadId | int | ID estado |
| EstadoNecesidadNombre | string | Nombre del estado |
| ModalidadTrabajoId | int | ID modalidad |
| ModalidadTrabajoNombre | string | Nombre modalidad |
| PresupuestoMin | decimal? | Presupuesto minimo |
| PresupuestoMax | decimal? | Presupuesto maximo |
| MonedaId | int? | ID moneda |
| MonedaNombre | string? | Codigo moneda |
| UbicacionCiudad | string? | Ciudad (si presencial/hibrido) |
| UbicacionPais | string? | Pais (si presencial/hibrido) |
| FechaLimitePropuestas | DateTime? | Fecha limite propuestas |
| FechaInicioPrevista | DateTime? | Fecha inicio prevista |
| FechaCreacion | DateTime | Fecha creacion |
| FechaActualizacion | DateTime? | Fecha actualizacion |
| ProyectoArtisticoId | Guid | ID proyecto asociado |
| ProyectoArtisticoNombre | string | Nombre del proyecto |
| Propuestas | List<PropuestaCrowdsourcingDto> | Lista de propuestas recibidas |

### 3.3 PropuestaCrowdsourcingDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/PropuestaCrowdsourcingDto.cs`

```csharp
namespace WePlayRises.Crowdsourcing.Application.Dtos;

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

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador de propuesta |
| ProfesionalId | Guid | ID del profesional que propone |
| ProfesionalNombre | string | Nombre del profesional |
| PrecioPropuesto | decimal | Precio propuesto |
| MonedaId | int | ID moneda del precio |
| TiempoEstimadoDias | int? | Dias estimados de trabajo |
| Mensaje | string | Mensaje/pitch del profesional |
| EstadoPropuestaId | int | ID estado (1=Pendiente, 2=Aceptada, 3=Rechazada) |
| EstadoPropuestaNombre | string | Nombre estado |
| FechaCreacion | DateTime | Fecha envio propuesta |

### 3.4 NecesidadCreateResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/NecesidadCreateResultDto.cs`

```csharp
namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class NecesidadCreateResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int EstadoNecesidadId { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
```

**Uso:** Response de `POST /necesidades`

### 3.5 NecesidadUpdateResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/NecesidadUpdateResultDto.cs`

```csharp
namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class NecesidadUpdateResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int EstadoNecesidadId { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public DateTime? FechaActualizacion { get; set; }
}
```

**Uso:** Response de `PUT /necesidades/{id}`

### 3.6 CerrarNecesidadResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/CerrarNecesidadResultDto.cs`

```csharp
namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class CerrarNecesidadResultDto
{
    public Guid Id { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public int PropuestasRechazadas { get; set; }
}
```

**Uso:** Response de `PATCH /necesidades/{id}/cerrar`

---

## 4. Commands y Queries

### 4.1 CreateNecesidadCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Commands/CreateNecesidadCommand.cs`

```csharp
using BuildingBlocks.Kernel.Http.Response;
using MediatR;
using WePlayRises.Crowdsourcing.Application.Dtos;

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;

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

**Handler en mismo archivo:** `CreateNecesidadCommandHandler`

### 4.2 UpdateNecesidadCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Commands/UpdateNecesidadCommand.cs`

```csharp
using BuildingBlocks.Kernel.Http.Response;
using MediatR;
using WePlayRises.Crowdsourcing.Application.Dtos;

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;

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

**Implementa:** `IRequest<ServiceResponse<NecesidadUpdateResultDto>>`

**Handler en mismo archivo:** `UpdateNecesidadCommandHandler`

### 4.3 CerrarNecesidadCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Commands/CerrarNecesidadCommand.cs`

```csharp
using BuildingBlocks.Kernel.Http.Response;
using MediatR;
using WePlayRises.Crowdsourcing.Application.Dtos;

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;

public class CerrarNecesidadCommand : IRequest<ServiceResponse<CerrarNecesidadResultDto>>
{
    public Guid Id { get; set; }
    public string? Motivo { get; set; }

    // Asignado desde JWT en Controller para validar ownership
    public Guid ArtistaId { get; set; }
}
```

**Implementa:** `IRequest<ServiceResponse<CerrarNecesidadResultDto>>`

**Handler en mismo archivo:** `CerrarNecesidadCommandHandler`

### 4.4 GetMisNecesidadesQuery

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Queries/GetMisNecesidadesQuery.cs`

```csharp
using BuildingBlocks.Kernel.Http.Response;
using MediatR;
using WePlayRises.Crowdsourcing.Application.Dtos;

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Queries;

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

**Handler en mismo archivo:** `GetMisNecesidadesQueryHandler`

### 4.5 GetNecesidadByIdQuery

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Queries/GetNecesidadByIdQuery.cs`

```csharp
using BuildingBlocks.Kernel.Http.Response;
using MediatR;
using WePlayRises.Crowdsourcing.Application.Dtos;

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Queries;

public class GetNecesidadByIdQuery : IRequest<ServiceResponse<NecesidadCrowdsourcingDto>>
{
    public Guid Id { get; set; }

    // Para validar ownership
    public Guid ArtistaId { get; set; }
}
```

**Implementa:** `IRequest<ServiceResponse<NecesidadCrowdsourcingDto>>`

**Handler en mismo archivo:** `GetNecesidadByIdQueryHandler`

---

## 5. Validadores

### 5.1 CreateNecesidadCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Validators/CreateNecesidadCommandValidator.cs`

```csharp
using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Validators;

public class CreateNecesidadCommandValidator : AbstractValidator<CreateNecesidadCommand>
{
    public CreateNecesidadCommandValidator(
        INecesidadCrowdsourcingService necesidadService,
        IProyectoArtisticoService proyectoService)
    {
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

        // Presupuesto Min/Max
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

        // ProyectoArtisticoId
        RuleFor(x => x.ProyectoArtisticoId)
            .NotEmpty()
            .WithMessage("El proyecto artístico es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MustAsync(async (command, proyectoId, ct) =>
            {
                var proyecto = await proyectoService.GetByIdAsync(proyectoId, ct);
                return proyecto != null && proyecto.ArtistaId == command.ArtistaId;
            })
            .WithMessage("El proyecto artístico no existe o no te pertenece")
            .WithErrorCode(ServiceResponseMessageType.Auth_Forbidden);
    }
}
```

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| Titulo | NotEmpty | El título es obligatorio | Validation_Required |
| Titulo | MinLength(5) | El título debe tener al menos 5 caracteres | Validation_MinLength |
| Titulo | MaxLength(200) | El título no puede superar los 200 caracteres | Validation_MaxLength |
| Descripcion | MaxLength(4000) | La descripción no puede superar los 4000 caracteres | Validation_MaxLength |
| TipoNecesidadId | NotEmpty | El tipo de necesidad es obligatorio | Validation_Required |
| ModalidadTrabajoId | NotEmpty | La modalidad de trabajo es obligatoria | Validation_Required |
| PresupuestoMin | GreaterThanOrEqualTo(0) | El presupuesto mínimo no puede ser negativo | Validation_InvalidRange |
| PresupuestoMax | GreaterThanOrEqualTo(min) | El presupuesto máximo debe ser mayor o igual al mínimo | Validation_InvalidRange |
| MonedaId | NotEmpty (si presupuesto) | La moneda es obligatoria cuando se especifica presupuesto | Validation_Required |
| UbicacionCiudad | NotEmpty (si presencial/hibrido) | La ubicación (ciudad) es obligatoria para modalidad Presencial o Híbrida | Validation_Required |
| UbicacionPais | NotEmpty (si presencial/hibrido) | La ubicación (país) es obligatoria para modalidad Presencial o Híbrida | Validation_Required |
| FechaLimitePropuestas | GreaterThan(Today) | La fecha límite debe ser posterior a hoy | Validation_InvalidDate |
| FechaInicioPrevista | GreaterThanOrEqualTo(Today) | La fecha de inicio debe ser igual o posterior a hoy | Validation_InvalidDate |
| ProyectoArtisticoId | NotEmpty + MustAsync | El proyecto artístico es obligatorio / no existe o no te pertenece | Validation_Required / Auth_Forbidden |

### 5.2 UpdateNecesidadCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Validators/UpdateNecesidadCommandValidator.cs`

```csharp
using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Validators;

public class UpdateNecesidadCommandValidator : AbstractValidator<UpdateNecesidadCommand>
{
    public UpdateNecesidadCommandValidator(INecesidadCrowdsourcingService necesidadService)
    {
        // Mismas validaciones que Create (excepto ProyectoArtisticoId y TipoNecesidadId)

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

        RuleFor(x => x.MonedaId)
            .NotEmpty()
            .When(x => x.PresupuestoMin.HasValue || x.PresupuestoMax.HasValue)
            .WithMessage("La moneda es obligatoria cuando se especifica presupuesto")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        // Ubicacion
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

        // Validar estado = Abierta (1)
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var necesidad = await necesidadService.GetByIdAsync(command.Id, ct);
                return necesidad != null
                    && necesidad.EstadoNecesidadId == 1
                    && necesidad.ArtistaId == command.ArtistaId;
            })
            .WithMessage("Solo se pueden editar necesidades en estado Abierta")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_NecesidadNotEditable);
    }
}
```

**Validacion critica adicional:** Estado debe ser "Abierta" (EstadoNecesidadId == 1)

### 5.3 CerrarNecesidadCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Validators/CerrarNecesidadCommandValidator.cs`

```csharp
using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Validators;

public class CerrarNecesidadCommandValidator : AbstractValidator<CerrarNecesidadCommand>
{
    public CerrarNecesidadCommandValidator(INecesidadCrowdsourcingService necesidadService)
    {
        // Motivo
        RuleFor(x => x.Motivo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Motivo))
            .WithMessage("El motivo no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        // Validar estado = Abierta (1) o En Progreso (2)
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var necesidad = await necesidadService.GetByIdAsync(command.Id, ct);
                return necesidad != null
                    && (necesidad.EstadoNecesidadId == 1 || necesidad.EstadoNecesidadId == 2)
                    && necesidad.ArtistaId == command.ArtistaId;
            })
            .WithMessage("Solo se pueden cerrar necesidades en estado Abierta o En Progreso")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_NecesidadNotCloseable);
    }
}
```

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| Motivo | MaxLength(500) | El motivo no puede superar los 500 caracteres | Validation_MaxLength |
| Necesidad | MustAsync (estado 1 o 2 + ownership) | Solo se pueden cerrar necesidades en estado Abierta o En Progreso | BusinessRule_NecesidadNotCloseable |

---

## 6. AutoMapper Mappings

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
            .ForMember(dest => dest.EstadoNecesidadId, opt => opt.MapFrom(src => 1)) // Abierta
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore());

        // Command -> Entity (para actualizar)
        CreateMap<UpdateNecesidadCommand, NecesidadCrowdsourcing>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ArtistaId, opt => opt.Ignore())
            .ForMember(dest => dest.TipoNecesidadId, opt => opt.Ignore()) // Inmutable
            .ForMember(dest => dest.ProyectoArtisticoId, opt => opt.Ignore()) // Inmutable
            .ForMember(dest => dest.EstadoNecesidadId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Entity -> ListDto
        CreateMap<NecesidadCrowdsourcing, NecesidadCrowdsourcingListDto>()
            .ForMember(dest => dest.EstadoNecesidadNombre, opt => opt.MapFrom(src => src.EstadoNecesidad.Nombre))
            .ForMember(dest => dest.TipoNecesidadNombre, opt => opt.MapFrom(src => src.TipoNecesidad.Nombre))
            .ForMember(dest => dest.ModalidadTrabajoNombre, opt => opt.MapFrom(src => src.ModalidadTrabajo.Nombre))
            .ForMember(dest => dest.MonedaNombre, opt => opt.MapFrom(src => src.Moneda != null ? src.Moneda.Codigo : null))
            .ForMember(dest => dest.NumeroPropuestas, opt => opt.MapFrom(src => src.Propuestas.Count));

        // Entity -> DTO (detalle completo)
        CreateMap<NecesidadCrowdsourcing, NecesidadCrowdsourcingDto>()
            .ForMember(dest => dest.EstadoNecesidadNombre, opt => opt.MapFrom(src => src.EstadoNecesidad.Nombre))
            .ForMember(dest => dest.TipoNecesidadNombre, opt => opt.MapFrom(src => src.TipoNecesidad.Nombre))
            .ForMember(dest => dest.ModalidadTrabajoNombre, opt => opt.MapFrom(src => src.ModalidadTrabajo.Nombre))
            .ForMember(dest => dest.MonedaNombre, opt => opt.MapFrom(src => src.Moneda != null ? src.Moneda.Codigo : null))
            .ForMember(dest => dest.ProyectoArtisticoNombre, opt => opt.MapFrom(src => src.ProyectoArtistico.Nombre))
            .ForMember(dest => dest.Propuestas, opt => opt.MapFrom(src => src.Propuestas));

        // Entity -> CreateResultDto
        CreateMap<NecesidadCrowdsourcing, NecesidadCreateResultDto>()
            .ForMember(dest => dest.EstadoNecesidadNombre, opt => opt.MapFrom(src => src.EstadoNecesidad.Nombre));

        // Entity -> UpdateResultDto
        CreateMap<NecesidadCrowdsourcing, NecesidadUpdateResultDto>()
            .ForMember(dest => dest.EstadoNecesidadNombre, opt => opt.MapFrom(src => src.EstadoNecesidad.Nombre));
    }
}
```

| Source | Destination | Notas |
|--------|-------------|-------|
| CreateNecesidadCommand | NecesidadCrowdsourcing | EstadoNecesidadId = 1 (Abierta), FechaCreacion = UtcNow |
| UpdateNecesidadCommand | NecesidadCrowdsourcing | TipoNecesidadId y ProyectoArtisticoId ignorados (inmutables) |
| NecesidadCrowdsourcing | NecesidadCrowdsourcingListDto | Include navigation properties para nombres |
| NecesidadCrowdsourcing | NecesidadCrowdsourcingDto | Include todas las nav properties + propuestas |
| NecesidadCrowdsourcing | NecesidadCreateResultDto | Mapping parcial para response |
| NecesidadCrowdsourcing | NecesidadUpdateResultDto | Mapping parcial para response |

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
        // Entity -> DTO
        CreateMap<PropuestaCrowdsourcing, PropuestaCrowdsourcingDto>()
            .ForMember(dest => dest.ProfesionalNombre, opt => opt.MapFrom(src => src.PerfilProfesional.NombreCompleto))
            .ForMember(dest => dest.EstadoPropuestaNombre, opt => opt.MapFrom(src => src.EstadoPropuesta.Nombre));
    }
}
```

| Source | Destination | Notas |
|--------|-------------|-------|
| PropuestaCrowdsourcing | PropuestaCrowdsourcingDto | Include PerfilProfesional y EstadoPropuesta para nombres |

---

## 7. Controller

### 7.1 NecesidadesCrowdsourcingController

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.WebApi/Controllers/NecesidadesCrowdsourcingController.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Queries;

namespace WePlayRises.Crowdsourcing.WebApi.Controllers;

[ApiController]
[Route("api/crowdsourcing/necesidades")]
[Authorize(Roles = "Artista")]
public class NecesidadesCrowdsourcingController : ControllerBase
{
    private readonly IMediator _mediator;

    public NecesidadesCrowdsourcingController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// POST /api/crowdsourcing/necesidades
    /// Crea una nueva necesidad de crowdsourcing
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ServiceResponse<NecesidadCreateResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateNecesidad(
        [FromBody] CreateNecesidadRequest request,
        CancellationToken cancellationToken)
    {
        var artistaId = GetArtistaIdFromToken();

        var command = new CreateNecesidadCommand
        {
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            TipoNecesidadId = request.TipoNecesidadId,
            ModalidadTrabajoId = request.ModalidadTrabajoId,
            PresupuestoMin = request.PresupuestoMin,
            PresupuestoMax = request.PresupuestoMax,
            MonedaId = request.MonedaId,
            UbicacionCiudad = request.UbicacionCiudad,
            UbicacionPais = request.UbicacionPais,
            FechaLimitePropuestas = request.FechaLimitePropuestas,
            FechaInicioPrevista = request.FechaInicioPrevista,
            ProyectoArtisticoId = request.ProyectoArtisticoId,
            ArtistaId = artistaId
        };

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetNecesidadById), new { id = result.Data!.Id }, result)
            : BadRequest(result);
    }

    /// <summary>
    /// GET /api/crowdsourcing/necesidades/mis-necesidades
    /// Lista necesidades del artista autenticado con paginación y filtros
    /// </summary>
    [HttpGet("mis-necesidades")]
    [ProducesResponseType(typeof(ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMisNecesidades(
        [FromQuery] int? estado,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var artistaId = GetArtistaIdFromToken();

        var query = new GetMisNecesidadesQuery
        {
            ArtistaId = artistaId,
            EstadoNecesidadId = estado,
            Search = search,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// GET /api/crowdsourcing/necesidades/{id}
    /// Obtiene detalle completo de una necesidad (solo propietario)
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServiceResponse<NecesidadCrowdsourcingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetNecesidadById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var artistaId = GetArtistaIdFromToken();

        var query = new GetNecesidadByIdQuery
        {
            Id = id,
            ArtistaId = artistaId
        };

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// PUT /api/crowdsourcing/necesidades/{id}
    /// Actualiza una necesidad (solo si estado = Abierta)
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ServiceResponse<NecesidadUpdateResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateNecesidad(
        Guid id,
        [FromBody] UpdateNecesidadRequest request,
        CancellationToken cancellationToken)
    {
        var artistaId = GetArtistaIdFromToken();

        var command = new UpdateNecesidadCommand
        {
            Id = id,
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            ModalidadTrabajoId = request.ModalidadTrabajoId,
            PresupuestoMin = request.PresupuestoMin,
            PresupuestoMax = request.PresupuestoMax,
            MonedaId = request.MonedaId,
            UbicacionCiudad = request.UbicacionCiudad,
            UbicacionPais = request.UbicacionPais,
            FechaLimitePropuestas = request.FechaLimitePropuestas,
            FechaInicioPrevista = request.FechaInicioPrevista,
            ArtistaId = artistaId
        };

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// PATCH /api/crowdsourcing/necesidades/{id}/cerrar
    /// Cierra una necesidad (auto-rechaza propuestas pendientes)
    /// </summary>
    [HttpPatch("{id:guid}/cerrar")]
    [ProducesResponseType(typeof(ServiceResponse<CerrarNecesidadResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CerrarNecesidad(
        Guid id,
        [FromBody] CerrarNecesidadRequest request,
        CancellationToken cancellationToken)
    {
        var artistaId = GetArtistaIdFromToken();

        var command = new CerrarNecesidadCommand
        {
            Id = id,
            Motivo = request.Motivo,
            ArtistaId = artistaId
        };

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    private Guid GetArtistaIdFromToken()
    {
        var artistaIdClaim = User.FindFirst("artistaId")?.Value
            ?? throw new UnauthorizedAccessException("ArtistaId no encontrado en token");

        return Guid.Parse(artistaIdClaim);
    }
}
```

**Nota:** El controller extrae `ArtistaId` desde el claim JWT `artistaId` en todas las acciones.

---

## 8. ServiceResponseMessageType - Nuevas Constantes

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`

**Agregar las siguientes constantes:**

```csharp
// Validation (1000-1999) - AGREGAR
public const string Validation_MinLength = "1011";
public const string Validation_InvalidDate = "1012";

// NotFound (2000-2999) - AGREGAR
public const string NotFound_Necesidad = "2009";

// Business Rules (4000-4999) - AGREGAR
public const string BusinessRule_NecesidadNotEditable = "4001";
public const string BusinessRule_NecesidadNotCloseable = "4002";
```

| Codigo | Constante | Uso |
|--------|-----------|-----|
| 1011 | Validation_MinLength | Titulo < 5 caracteres |
| 1012 | Validation_InvalidDate | Fecha invalida (limite o inicio) |
| 2009 | NotFound_Necesidad | Necesidad no encontrada |
| 4001 | BusinessRule_NecesidadNotEditable | Solo se edita si estado = Abierta |
| 4002 | BusinessRule_NecesidadNotCloseable | Solo se cierra si estado = Abierta o EnProgreso |

**Archivo completo actualizado:**

```csharp
namespace WePlayRises.Crowdsourcing.Domain.Constants;

public static class ServiceResponseMessageType
{
    // Success (0000-0999)
    public const string Success = "0000";
    public const string Created = "0001";
    public const string Updated = "0002";
    public const string Deleted = "0003";

    // Validation (1000-1999)
    public const string Validation_Required = "1001";
    public const string Validation_MaxLength = "1002";
    public const string Validation_InvalidEmail = "1003";
    public const string Validation_InvalidRange = "1009";
    public const string Validation_ForeignKeyNotFound = "1010";
    public const string Validation_MinLength = "1011";  // NUEVO
    public const string Validation_InvalidDate = "1012";  // NUEVO

    // NotFound (2000-2999)
    public const string NotFound_Entity = "2000";
    public const string NotFound_PlantillaProyecto = "2006";
    public const string NotFound_ProyectoArtistico = "2007";
    public const string NotFound_PlantillaNecesidad = "2008";
    public const string NotFound_Necesidad = "2009";  // NUEVO

    // Auth (3000-3999)
    public const string Auth_Unauthorized = "3001";
    public const string Auth_Forbidden = "3002";

    // Business Rules (4000-4999)
    public const string BusinessRule_InvalidOperation = "4000";
    public const string BusinessRule_NecesidadNotEditable = "4001";  // NUEVO
    public const string BusinessRule_NecesidadNotCloseable = "4002";  // NUEVO

    // Internal (5000-5999)
    public const string Internal_UnexpectedError = "5000";
    public const string Internal_DatabaseError = "5001";
}
```

---

## 9. OpenAPI Documentation

### POST /api/crowdsourcing/necesidades

- **Summary:** Crear nueva necesidad de crowdsourcing
- **Tags:** Crowdsourcing - Necesidades
- **Request Body:** CreateNecesidadRequest (application/json)
- **Responses:**
  - **201 Created:** ServiceResponse<NecesidadCreateResultDto>
    - Headers: `Location: /api/crowdsourcing/necesidades/{id}`
  - **400 Bad Request:** ServiceResponse con lista de errores de validacion
  - **401 Unauthorized:** Token invalido o expirado
  - **403 Forbidden:** El proyecto no pertenece al artista autenticado
  - **404 Not Found:** TipoNecesidad, ModalidadTrabajo, Moneda o ProyectoArtistico no existe
  - **500 Internal Server Error:** Error inesperado
- **Security:** Bearer JWT (claim `artistaId` requerido)

### GET /api/crowdsourcing/necesidades/mis-necesidades

- **Summary:** Listar mis necesidades con paginacion y filtros
- **Tags:** Crowdsourcing - Necesidades
- **Parameters:**
  - `estado` (query, int, optional): Filtrar por EstadoNecesidadId (1=Abierta, 2=EnProgreso, 3=Cerrada, 4=Cancelada)
  - `search` (query, string, optional): Buscar en titulo y descripcion
  - `page` (query, int, default: 1): Numero de pagina
  - `pageSize` (query, int, default: 12, max: 50): Items por pagina
- **Responses:**
  - **200 OK:** ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>
  - **401 Unauthorized:** Token invalido o expirado
  - **500 Internal Server Error:** Error inesperado
- **Security:** Bearer JWT

### GET /api/crowdsourcing/necesidades/{id}

- **Summary:** Obtener detalle completo de necesidad
- **Tags:** Crowdsourcing - Necesidades
- **Parameters:**
  - `id` (path, Guid, required): ID de la necesidad
- **Responses:**
  - **200 OK:** ServiceResponse<NecesidadCrowdsourcingDto>
  - **401 Unauthorized:** Token invalido o expirado
  - **403 Forbidden:** La necesidad no pertenece al artista autenticado
  - **404 Not Found:** Necesidad no existe
  - **500 Internal Server Error:** Error inesperado
- **Security:** Bearer JWT

### PUT /api/crowdsourcing/necesidades/{id}

- **Summary:** Actualizar necesidad (solo si estado = Abierta)
- **Tags:** Crowdsourcing - Necesidades
- **Parameters:**
  - `id` (path, Guid, required): ID de la necesidad
- **Request Body:** UpdateNecesidadRequest (application/json)
- **Responses:**
  - **200 OK:** ServiceResponse<NecesidadUpdateResultDto>
  - **400 Bad Request:** Validacion fallida o estado != Abierta
  - **401 Unauthorized:** Token invalido o expirado
  - **403 Forbidden:** La necesidad no pertenece al artista autenticado
  - **404 Not Found:** Necesidad no existe
  - **500 Internal Server Error:** Error inesperado
- **Security:** Bearer JWT

### PATCH /api/crowdsourcing/necesidades/{id}/cerrar

- **Summary:** Cerrar necesidad (rechaza propuestas pendientes)
- **Tags:** Crowdsourcing - Necesidades
- **Parameters:**
  - `id` (path, Guid, required): ID de la necesidad
- **Request Body:** CerrarNecesidadRequest (application/json)
- **Responses:**
  - **200 OK:** ServiceResponse<CerrarNecesidadResultDto>
  - **400 Bad Request:** Estado no permite cierre (solo Abierta/EnProgreso)
  - **401 Unauthorized:** Token invalido o expirado
  - **403 Forbidden:** La necesidad no pertenece al artista autenticado
  - **404 Not Found:** Necesidad no existe
  - **500 Internal Server Error:** Error inesperado
- **Security:** Bearer JWT

---

## 10. Archivos a Crear

```
Modules/Crowdsourcing/
├── WePlayRises.Crowdsourcing.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs        [MODIFICAR - agregar 1011, 1012, 2009, 4001, 4002]
│
├── WePlayRises.Crowdsourcing.Application/
│   ├── Dtos/
│   │   ├── CreateNecesidadRequest.cs            [CREAR]
│   │   ├── UpdateNecesidadRequest.cs            [CREAR]
│   │   ├── CerrarNecesidadRequest.cs            [CREAR]
│   │   ├── NecesidadCrowdsourcingListDto.cs     [CREAR]
│   │   ├── NecesidadCrowdsourcingDto.cs         [CREAR]
│   │   ├── PropuestaCrowdsourcingDto.cs         [CREAR]
│   │   ├── NecesidadCreateResultDto.cs          [CREAR]
│   │   ├── NecesidadUpdateResultDto.cs          [CREAR]
│   │   └── CerrarNecesidadResultDto.cs          [CREAR]
│   │
│   ├── Features/Necesidades/
│   │   ├── Commands/
│   │   │   ├── CreateNecesidadCommand.cs        [CREAR - Command + Handler]
│   │   │   ├── UpdateNecesidadCommand.cs        [CREAR - Command + Handler]
│   │   │   └── CerrarNecesidadCommand.cs        [CREAR - Command + Handler]
│   │   │
│   │   ├── Queries/
│   │   │   ├── GetMisNecesidadesQuery.cs        [CREAR - Query + Handler]
│   │   │   └── GetNecesidadByIdQuery.cs         [CREAR - Query + Handler]
│   │   │
│   │   └── Validators/
│   │       ├── CreateNecesidadCommandValidator.cs   [CREAR]
│   │       ├── UpdateNecesidadCommandValidator.cs   [CREAR]
│   │       └── CerrarNecesidadCommandValidator.cs   [CREAR]
│   │
│   ├── Mapping/
│   │   ├── NecesidadCrowdsourcingProfile.cs     [CREAR]
│   │   └── PropuestaCrowdsourcingProfile.cs     [CREAR]
│   │
│   └── Interfaces/Services/
│       └── IProyectoArtisticoService.cs         [VERIFICAR - puede estar en Crowdfunding o UserAccess]
│
└── WePlayRises.Crowdsourcing.WebApi/
    └── Controllers/
        └── NecesidadesCrowdsourcingController.cs   [CREAR]
```

**Total de archivos:**
- **Modificar:** 1 archivo (ServiceResponseMessageType.cs)
- **Crear:** 21 archivos nuevos

---

## 11. Checklist

- [ ] Request DTOs (CreateNecesidadRequest, UpdateNecesidadRequest, CerrarNecesidadRequest)
- [ ] Response DTOs (NecesidadCrowdsourcingListDto, NecesidadCrowdsourcingDto, PropuestaCrowdsourcingDto, Result DTOs)
- [ ] Commands con IRequest<ServiceResponse<T>> (CreateNecesidadCommand, UpdateNecesidadCommand, CerrarNecesidadCommand)
- [ ] Handlers en mismo archivo que Command/Query
- [ ] Queries (GetMisNecesidadesQuery, GetNecesidadByIdQuery)
- [ ] Validators con WithMessage + WithErrorCode usando ServiceResponseMessageType
- [ ] Validacion condicional: moneda requerida si presupuesto, ubicacion requerida si presencial/hibrido
- [ ] Validacion de estado (Abierta para editar, Abierta/EnProgreso para cerrar)
- [ ] Validacion de ownership (ProyectoArtistico pertenece a ArtistaId del JWT)
- [ ] AutoMapper profiles (NecesidadCrowdsourcingProfile, PropuestaCrowdsourcingProfile)
- [ ] Mapping Command -> Entity con transformaciones (estado=Abierta en create, FechaActualizacion en update)
- [ ] Mapping Entity -> DTO con navigation properties (nombres de maestras)
- [ ] Controller con [Authorize(Roles = "Artista")]
- [ ] Controller extrae ArtistaId desde JWT en todos los endpoints
- [ ] ServiceResponseMessageType constants (1011, 1012, 2009, 4001, 4002)
- [ ] OpenAPI documentation con summaries y response types
- [ ] Handlers con try-catch y logging
- [ ] Handlers con ?? throw en constructores (service, mapper, validator, logger)

---

## 12. Siguiente Paso Sugerido

**Orden de implementacion:**

1. **ServiceResponseMessageType** - Agregar nuevas constantes (1011, 1012, 2009, 4001, 4002)
2. **DTOs** - Crear todos los DTOs (Request, Response, Result)
3. **Commands/Queries** - Crear Commands y Queries (sin Handlers aun)
4. **Validators** - Crear validadores con todas las reglas
5. **Handlers** - Implementar Handlers en mismo archivo que Command/Query
6. **AutoMapper** - Crear profiles de mapping
7. **Controller** - Crear controller con endpoints
8. **Testing** - Unit tests de validators y handlers

**Comandos:**

```bash
# Compilar modulo Crowdsourcing
cd src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application
dotnet build

# Ejecutar tests
cd ../../../../tests/Crowdsourcing.Application.Tests
dotnet test

# Verificar API swagger
cd ../../../src/api
dotnet run --project WebApi
# Abrir: https://localhost:5001/swagger
```

**Dependencias bloqueantes:**

- **INecesidadCrowdsourcingService** - Ya existe (verificado en Glob)
- **IProyectoArtisticoService** - Verificar si existe en Crowdfunding o UserAccess
- **Maestras** - TipoNecesidad, EstadoNecesidad, ModalidadTrabajo, Moneda (verificar seeds)

---

**Fin del Plan de Contratos API**
