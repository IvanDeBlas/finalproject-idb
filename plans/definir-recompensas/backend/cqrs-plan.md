# Plan CQRS: Definir Recompensas

**Fecha:** 2026-02-13
**Modulo:** Crowdfunding
**Feature:** definir-recompensas

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Request | Response |
|-----------|------|---------|----------|
| Crear Recompensa | Command | CreateRewardCommand | ServiceResponse\<RewardDto\> |
| Actualizar Recompensa | Command | UpdateRewardCommand | ServiceResponse\<bool\> |
| Eliminar Recompensa | Command | DeleteRewardCommand | ServiceResponse\<bool\> |
| Reordenar Recompensas | Command | ReorderRewardsCommand | ServiceResponse\<bool\> |
| Obtener por ID | Query | GetRewardByIdQuery | ServiceResponse\<RewardDto\> |
| Listar con filtros | Query | GetAllRewardsQuery | ServiceResponse\<IEnumerable\<RewardListDto\>\> |

---

## 2. Commands

### 2.1 CreateRewardCommand (EXISTE - ACTUALIZAR Handler)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Commands/CreateRewardCommand.cs`

**Estado:** ✅ YA EXISTE - Requiere **MODIFICAR Handler** para agregar validaciones de ownership y auto-incrementar orden

**Contiene:** Command + Handler (mismo archivo)

#### Command (Sin cambios)

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| CampaniaId | Guid | ID de la campania |
| TipoRewardId | int | Tipo de recompensa (1=Digital, 2=Fisico, 3=Experiencia, 4=Otro) |
| Nombre | string | Nombre de la recompensa (max 200) |
| Descripcion | string? | Descripcion detallada (max 2000, opcional) |
| ImporteMinimo | decimal | Monto minimo de aportacion (> 0) |
| MonedaId | int | Moneda (1=EUR por defecto) |
| EsAddOn | bool | Si es complemento opcional |
| CantidadMaxima | int? | Stock limitado (null = ilimitado) |
| CantidadPorBacker | int? | Limite por backer |
| IncluyeEnvioFisico | bool | Requiere direccion de envio |
| TiempoEntregaEstimado | string? | Texto libre (max 200) |
| Orden | int | Orden de visualizacion (>= 0) |

**Implementa:** `IRequest<ServiceResponse<RewardDto>>`

#### Handler (MODIFICAR)

**Dependencias actuales (OK):**
- `IRewardService` - Para persistencia
- `ICampaniaService` - Para validar que campania existe y ownership
- `IMapper` - Para mappings
- `IValidator<CreateRewardCommand>` - Para validacion
- `ILogger<CreateRewardCommandHandler>` - Para logging

**Flujo actual:**
1. Validar request con FluentValidation ✅
2. Verificar que campania existe ✅
3. Mapear Command -> Reward (entity) ✅
4. Setear Id, EsActivo=true, FechaCreacion ✅
5. Crear via service ✅
6. Obtener entidad creada y mapear a DTO ✅
7. Retornar ServiceResponse con DTO ✅
8. Try-catch con logging ✅

**CAMBIOS REQUERIDOS en Handler (despues de linea 86, antes del mapeo):**

```csharp
// AGREGAR despues de validar que campania existe (linea 86):

// 2.1. Validar ownership (ArtistaId del token debe coincidir con ArtistaId de la campania)
// TODO: Implementar extraccion de ArtistaId desde JWT claims via IHttpContextAccessor
// Ejemplo:
// var artistaIdFromToken = GetArtistaIdFromToken();
// var campania = await _campaniaService.GetByIdAsync(campaniaId, cancellationToken);
// if (campania.ArtistaId.Value != artistaIdFromToken)
// {
//     return new ServiceResponse<RewardDto>
//     {
//         Messages = new List<ServiceResponseMessage>
//         {
//             new() { Message = "No tienes permiso para crear recompensas en esta campania", ErrorCode = ServiceResponseMessageType.Auth_Forbidden }
//         }
//     };
// }

// 3. Mapear a entidad de dominio (MANTENER linea 89-92)
var entity = _mapper.Map<CampaniaCrowdfundingReward>(request);
entity.Id = CampaniaCrowdfundingRewardId.CreateNew();
entity.EsActivo = true;
entity.FechaCreacion = DateTime.UtcNow;

// AGREGAR despues de linea 92 (antes de CreateAsync):

// 3.1. Auto-incrementar Orden si no se proporciono o es 0
if (entity.Orden == 0)
{
    var maxOrden = await _rewardService.GetMaxOrdenAsync(campaniaId, cancellationToken);
    entity.Orden = maxOrden + 1;
}

// 4. Crear via servicio (MANTENER linea 95-106)
```

**Notas:**
- La validacion de ownership requiere inyectar `IHttpContextAccessor` para acceder a los claims del JWT
- El metodo `GetMaxOrdenAsync()` debe existir en `IRewardService` (ver seccion 6)
- NO modificar el Validator (ya tiene todas las validaciones necesarias)

---

### 2.2 UpdateRewardCommand (EXISTE - ACTUALIZAR Handler)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Commands/UpdateRewardCommand.cs`

**Estado:** ✅ YA EXISTE - Requiere **MODIFICAR Handler** para agregar validaciones de ownership y backings

**Contiene:** Command + Handler (mismo archivo)

#### Command (Sin cambios)

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| Id | Guid | Si | ID de la recompensa |
| Nombre | string? | No | Nombre (max 200) |
| Descripcion | string? | No | Descripcion (max 2000) |
| ImporteMinimo | decimal? | No | Importe minimo (> 0) |
| TipoRewardId | int? | No | Tipo de recompensa |
| MonedaId | int? | No | Moneda |
| EsAddOn | bool? | No | Es complemento |
| CantidadMaxima | int? | No | Stock limitado |
| CantidadPorBacker | int? | No | Limite por backer |
| IncluyeEnvioFisico | bool? | No | Requiere envio |
| TiempoEntregaEstimado | string? | No | Tiempo de entrega (max 200) |
| Orden | int? | No | Orden (>= 0) |
| EsActivo | bool? | No | Estado activo/inactivo |

**Implementa:** `IRequest<ServiceResponse<bool>>`

#### Handler (MODIFICAR)

**Dependencias actuales (OK):**
- `IRewardService` - Para persistencia y validaciones
- `IValidator<UpdateRewardCommand>` - Para validacion
- `ILogger<UpdateRewardCommandHandler>` - Para logging

**Dependencias a AGREGAR:**
- `ICampaniaService` - Para validar ownership

**Flujo actual:**
1. Validar request con FluentValidation ✅
2. Obtener entidad existente ✅
3. Verificar que existe (NotFound si null) ✅
4. Actualizar propiedades proporcionadas (PATCH semantics) ✅
5. Setear FechaActualizacion ✅
6. Actualizar via service ✅
7. Retornar ServiceResponse con true ✅
8. Try-catch con logging ✅

**CAMBIOS REQUERIDOS en Handler:**

```csharp
// MODIFICAR constructor para inyectar ICampaniaService:
private readonly IRewardService _service;
private readonly ICampaniaService _campaniaService; // NUEVO
private readonly IValidator<UpdateRewardCommand> _validator;
private readonly ILogger<UpdateRewardCommandHandler> _logger;

public UpdateRewardCommandHandler(
    IRewardService service,
    ICampaniaService campaniaService, // NUEVO
    IValidator<UpdateRewardCommand> validator,
    ILogger<UpdateRewardCommandHandler> logger)
{
    _service = service ?? throw new ArgumentNullException(nameof(service));
    _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService)); // NUEVO
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}

// AGREGAR despues de verificar que entity != null (linea 80, despues del NotFound check):

// 3. Validar ownership (ArtistaId del token debe coincidir con ArtistaId de la campania)
// TODO: Implementar extraccion de ArtistaId desde JWT claims via IHttpContextAccessor
// var artistaIdFromToken = GetArtistaIdFromToken();
// var campania = await _campaniaService.GetByIdAsync(entity.CampaniaId, cancellationToken);
// if (campania.ArtistaId.Value != artistaIdFromToken)
// {
//     return new ServiceResponse<bool>
//     {
//         Messages = new List<ServiceResponseMessage>
//         {
//             new() { Message = "No tienes permiso para modificar esta recompensa", ErrorCode = ServiceResponseMessageType.Auth_Forbidden }
//         }
//     };
// }

// 4. Validar que reward NO tiene backings antes de modificar campos criticos
var hasBackings = await _service.HasBackingsAsync(rewardId, cancellationToken);
if (hasBackings)
{
    // Si tiene backings, solo permitir editar Descripcion, TiempoEntregaEstimado y EsActivo
    if (request.Nombre != null || request.ImporteMinimo.HasValue ||
        request.CantidadMaxima.HasValue || request.TipoRewardId.HasValue ||
        request.MonedaId.HasValue || request.EsAddOn.HasValue ||
        request.CantidadPorBacker.HasValue || request.IncluyeEnvioFisico.HasValue ||
        request.Orden.HasValue)
    {
        return new ServiceResponse<bool>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "No se puede modificar campos criticos de una recompensa con aportes existentes. Solo puedes editar Descripcion, Tiempo de Entrega y Estado.",
                    ErrorCode = ServiceResponseMessageType.BusinessRule_RewardHasBackings
                }
            }
        };
    }
}

// 5. Actualizar solo propiedades proporcionadas (MANTENER lineas 83-108)
```

**Notas:**
- Si reward tiene backings, SOLO permitir editar: `Descripcion`, `TiempoEntregaEstimado`, `EsActivo`
- Campos bloqueados si hay backings: `Nombre`, `ImporteMinimo`, `CantidadMaxima`, `TipoRewardId`, `MonedaId`, `EsAddOn`, `CantidadPorBacker`, `IncluyeEnvioFisico`, `Orden`
- El metodo `HasBackingsAsync()` debe existir en `IRewardService` (ver seccion 6)

---

### 2.3 DeleteRewardCommand (EXISTE - ACTUALIZAR Handler)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Commands/DeleteRewardCommand.cs`

**Estado:** ✅ YA EXISTE - Requiere **MODIFICAR Handler completo** para agregar validaciones de ownership y backings

**Contiene:** Command + Handler (mismo archivo)

#### Command (Sin cambios esperados)

| Propiedad | Tipo | Requerido |
|-----------|------|-----------|
| Id | Guid | Si |

**Implementa:** `IRequest<ServiceResponse<bool>>`

#### Handler (REESCRIBIR)

**Dependencias esperadas:**
- `IRewardService` - Para persistencia y validaciones
- `ICampaniaService` - Para validar ownership
- `ILogger<DeleteRewardCommandHandler>` - Para logging

**Flujo completo:**
1. Obtener entidad existente
2. Verificar que existe (NotFound si null)
3. Validar ownership (ArtistaId del token coincide con ArtistaId de la campania)
4. **CRITICO:** Validar que reward NO tiene backings (retornar error 4010 si tiene)
5. Hacer soft delete (EsActivo = false)
6. Setear FechaActualizacion
7. Actualizar via service
8. Retornar ServiceResponse con true
9. Try-catch con logging

**Handler completo esperado:**

```csharp
using Microsoft.Extensions.Logging;
using MediatR;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public record DeleteRewardCommand(Guid Id) : IRequest<ServiceResponse<bool>>;

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class DeleteRewardCommandHandler : IRequestHandler<DeleteRewardCommand, ServiceResponse<bool>>
{
    private readonly IRewardService _rewardService;
    private readonly ICampaniaService _campaniaService;
    private readonly ILogger<DeleteRewardCommandHandler> _logger;

    public DeleteRewardCommandHandler(
        IRewardService rewardService,
        ICampaniaService campaniaService,
        ILogger<DeleteRewardCommandHandler> logger)
    {
        _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<bool>> Handle(
        DeleteRewardCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Obtener entidad existente
            var rewardId = new CampaniaCrowdfundingRewardId(request.Id);
            var entity = await _rewardService.GetByIdAsync(rewardId, cancellationToken);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<bool>(
                    "Reward no encontrado",
                    ServiceResponseMessageType.NotFound_Reward);
            }

            // 2. Validar ownership (ArtistaId del token debe coincidir con ArtistaId de la campania)
            // TODO: Implementar extraccion de ArtistaId desde JWT claims via IHttpContextAccessor
            // var artistaIdFromToken = GetArtistaIdFromToken();
            // var campania = await _campaniaService.GetByIdAsync(entity.CampaniaId, cancellationToken);
            // if (campania.ArtistaId.Value != artistaIdFromToken)
            // {
            //     return new ServiceResponse<bool>
            //     {
            //         Messages = new List<ServiceResponseMessage>
            //         {
            //             new() { Message = "No tienes permiso para eliminar esta recompensa", ErrorCode = ServiceResponseMessageType.Auth_Forbidden }
            //         }
            //     };
            // }

            // 3. CRITICO: Validar que reward NO tiene backings
            var hasBackings = await _rewardService.HasBackingsAsync(rewardId, cancellationToken);
            if (hasBackings)
            {
                return new ServiceResponse<bool>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "No se puede eliminar una recompensa con aportes existentes",
                            ErrorCode = ServiceResponseMessageType.BusinessRule_RewardHasBackings
                        }
                    }
                };
            }

            // 4. Soft delete
            entity.EsActivo = false;
            entity.FechaActualizacion = DateTime.UtcNow;

            // 5. Actualizar via servicio
            await _rewardService.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("Reward soft deleted with Id {Id}", request.Id);

            return new ServiceResponse<bool>
            {
                Data = true,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Reward eliminado correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Reward with Id {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<bool>(
                "Error inesperado al eliminar reward",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
```

**Notas:**
- **NUNCA** usar `DeleteAsync()` del service, siempre soft delete con `UpdateAsync()`
- Si reward tiene backings, retornar error `BusinessRule_RewardHasBackings` (codigo 4010)
- NO inyectar `IValidator` (DELETE no requiere validacion de request body)

---

### 2.4 ReorderRewardsCommand (NUEVO)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Commands/ReorderRewardsCommand.cs`

**Estado:** ❌ NUEVO - A CREAR

**Contiene:** DTO + Command + Handler (mismo archivo)

#### RewardOrderDto (Clase auxiliar)

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| RewardId | Guid | ID del reward |
| Orden | int | Nuevo orden (>= 0) |

#### Command

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| CampaniaId | Guid | ID de la campania |
| RewardOrders | List\<RewardOrderDto\> | Lista de reordenamientos |

**Implementa:** `IRequest<ServiceResponse<bool>>`

#### Handler

**Dependencias:**
- `IRewardService` - Para reordenar y validar rewards
- `ICampaniaService` - Para validar que campania existe y ownership
- `IValidator<ReorderRewardsCommand>` - Para validacion
- `ILogger<ReorderRewardsCommandHandler>` - Para logging

**Flujo:**
1. Validar request con FluentValidation
2. Verificar que campania existe
3. Validar ownership (ArtistaId del token coincide con ArtistaId de la campania)
4. Validar que todos los rewards existen y pertenecen a la campania
5. Llamar `_rewardService.ReorderAsync()` con lista de (RewardId, Orden)
6. Retornar ServiceResponse con true
7. Try-catch con logging

#### Contenido Completo del Archivo

```csharp
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;

// -----------------------------------------------------------------------------
// DTO
// -----------------------------------------------------------------------------
public class RewardOrderDto
{
    public Guid RewardId { get; set; }
    public int Orden { get; set; }
}

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class ReorderRewardsCommand : IRequest<ServiceResponse<bool>>
{
    public Guid CampaniaId { get; set; }
    public List<RewardOrderDto> RewardOrders { get; set; } = new();
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class ReorderRewardsCommandHandler : IRequestHandler<ReorderRewardsCommand, ServiceResponse<bool>>
{
    private readonly IRewardService _rewardService;
    private readonly ICampaniaService _campaniaService;
    private readonly IValidator<ReorderRewardsCommand> _validator;
    private readonly ILogger<ReorderRewardsCommandHandler> _logger;

    public ReorderRewardsCommandHandler(
        IRewardService rewardService,
        ICampaniaService campaniaService,
        IValidator<ReorderRewardsCommand> validator,
        ILogger<ReorderRewardsCommandHandler> logger)
    {
        _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<bool>> Handle(
        ReorderRewardsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar comando
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for ReorderRewards: {Errors}",
                    string.Join(", ", validationResult.Errors));

                return new ServiceResponse<bool>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Verificar que la campania existe
            var campaniaId = new CampaniaCrowdfundingId(request.CampaniaId);
            var campaniaExists = await _campaniaService.ExistsAsync(campaniaId, cancellationToken);
            if (!campaniaExists)
            {
                return ValidateExtensions.NotFoundServiceResponse<bool>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            // 3. Validar ownership (ArtistaId del token debe coincidir con ArtistaId de la campania)
            // TODO: Implementar extraccion de ArtistaId desde JWT claims via IHttpContextAccessor
            // var artistaIdFromToken = GetArtistaIdFromToken();
            // var campania = await _campaniaService.GetByIdAsync(campaniaId, cancellationToken);
            // if (campania.ArtistaId.Value != artistaIdFromToken)
            // {
            //     return new ServiceResponse<bool>
            //     {
            //         Messages = new List<ServiceResponseMessage>
            //         {
            //             new() { Message = "No tienes permiso para reordenar recompensas en esta campania", ErrorCode = ServiceResponseMessageType.Auth_Forbidden }
            //         }
            //     };
            // }

            // 4. Validar que todos los rewards existen y pertenecen a la campania
            foreach (var order in request.RewardOrders)
            {
                var rewardId = new CampaniaCrowdfundingRewardId(order.RewardId);
                var reward = await _rewardService.GetByIdAsync(rewardId, cancellationToken);

                if (reward == null)
                {
                    return ValidateExtensions.NotFoundServiceResponse<bool>(
                        $"Reward {order.RewardId} no encontrado",
                        ServiceResponseMessageType.NotFound_Reward);
                }

                if (reward.CampaniaId.Value != request.CampaniaId)
                {
                    return new ServiceResponse<bool>
                    {
                        Messages = new List<ServiceResponseMessage>
                        {
                            new()
                            {
                                Message = "Uno o mas rewards no pertenecen a la campania especificada",
                                ErrorCode = ServiceResponseMessageType.BusinessRule_OperationNotAllowed
                            }
                        }
                    };
                }
            }

            // 5. Convertir a tuplas (RewardId, Orden) para el service
            var updates = request.RewardOrders
                .Select(o => (new CampaniaCrowdfundingRewardId(o.RewardId), o.Orden))
                .ToList();

            // 6. Reordenar via servicio
            await _rewardService.ReorderAsync(campaniaId, updates, cancellationToken);

            _logger.LogInformation("Rewards reordenados exitosamente para Campania {CampaniaId}", request.CampaniaId);

            return new ServiceResponse<bool>
            {
                Data = true,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Recompensas reordenadas correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering rewards for Campania {CampaniaId}", request.CampaniaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<bool>(
                "Error inesperado al reordenar rewards",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
```

**Notas:**
- El service method `ReorderAsync()` debe implementarse en `IRewardService` (ver seccion 6)
- Validar que TODOS los rewards pertenecen a la campania especificada antes de reordenar
- Bulk update: actualizar todos los ordenes en una sola transaccion (repository hace SaveChanges al final)

---

## 3. Queries

### 3.1 GetAllRewardsQuery (EXISTE - VERIFICAR)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Queries/GetAllRewardsQuery.cs`

**Estado:** ✅ YA EXISTE - Verificar que ordena por campo `Orden ASC`

**Contiene:** Query + Handler (mismo archivo)

#### Query

| Query Param | Tipo | Requerido | Descripcion |
|-------------|------|-----------|-------------|
| CampaniaId | Guid? | No | Filtrar por campania |
| EsActivo | bool? | No | Filtrar por estado activo/inactivo |
| EsAddOn | bool? | No | Filtrar por tipo add-on |

**Implementa:** `IRequest<ServiceResponse<IEnumerable<RewardListDto>>>`

#### Handler

**Verificar que el Handler:**
- Llama `_rewardService.GetByCampaniaIdAsync()` si campaniaId != null
- Llama `_rewardService.GetAllAsync()` si campaniaId == null
- **ORDENA los resultados por campo `Orden` ASC antes de mapear a DTOs**
- Mapea entidades a `RewardListDto` usando AutoMapper

**Cambios esperados:**

```csharp
// Despues de obtener entidades del service, AGREGAR ordenamiento:
var entities = await _rewardService.GetByCampaniaIdAsync(campaniaId, cancellationToken);

// AGREGAR ordenamiento antes de mapear:
var orderedEntities = entities.OrderBy(r => r.Orden).ToList();

// Mapear a DTOs
var dtos = _mapper.Map<IEnumerable<RewardListDto>>(orderedEntities);
```

---

### 3.2 GetRewardByIdQuery (EXISTE - OK)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Queries/GetRewardByIdQuery.cs`

**Estado:** ✅ YA EXISTE - Sin cambios requeridos

**Contiene:** Query + Handler (mismo archivo)

#### Query

| Propiedad | Tipo | Requerido |
|-----------|------|-----------|
| Id | Guid | Si |

**Implementa:** `IRequest<ServiceResponse<RewardDto>>`

#### Handler

**Dependencias:**
- `IRewardService` - Para obtener reward
- `IMapper` - Para mapear a DTO
- `ILogger` - Para logging

**Flujo:**
1. Obtener reward por ID via service
2. Si null, retornar NotFound con codigo 2004
3. Mapear entidad a `RewardDto`
4. Retornar ServiceResponse con DTO
5. Try-catch con logging

**Sin cambios requeridos.**

---

## 4. Validators

### 4.1 CreateRewardCommandValidator (EXISTE - OK)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Validators/CreateRewardCommandValidator.cs`

**Estado:** ✅ YA EXISTE - Sin cambios requeridos

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| CampaniaId | NotEmpty | El CampaniaId es obligatorio | Validation_Required |
| Nombre | NotEmpty | El nombre es obligatorio | Validation_Required |
| Nombre | MaxLength(200) | El nombre no puede superar los 200 caracteres | Validation_MaxLength |
| Descripcion | MaxLength(2000) | La descripcion no puede superar los 2000 caracteres | Validation_MaxLength |
| ImporteMinimo | GreaterThan(0) | El importe minimo debe ser mayor a 0 | Validation_InvalidAmount |
| MonedaId | GreaterThan(0) | La moneda es obligatoria | Validation_Required |
| TipoRewardId | GreaterThan(0) | El tipo de reward es obligatorio | Validation_Required |
| CantidadMaxima | GreaterThan(0) (When HasValue) | La cantidad maxima debe ser mayor a 0 | Validation_InvalidRange |
| CantidadPorBacker | GreaterThan(0) (When HasValue) | La cantidad por backer debe ser mayor a 0 | Validation_InvalidRange |
| TiempoEntregaEstimado | MaxLength(200) (When not null) | El tiempo de entrega estimado no puede superar los 200 caracteres | Validation_MaxLength |
| Orden | GreaterThanOrEqualTo(0) | El orden debe ser mayor o igual a 0 | Validation_InvalidRange |

**Notas:**
- **NO modificar este archivo** - Ya tiene todas las validaciones necesarias
- La validacion de ownership se hace en el **Handler**, NO en el validator

---

### 4.2 UpdateRewardCommandValidator (EXISTE - OK)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Validators/UpdateRewardCommandValidator.cs`

**Estado:** ✅ YA EXISTE - Sin cambios requeridos

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| Id | NotEmpty | El Id es obligatorio | Validation_Required |
| Nombre | MaxLength(200) (When not null) | El nombre no puede superar los 200 caracteres | Validation_MaxLength |
| Descripcion | MaxLength(2000) (When not null) | La descripcion no puede superar los 2000 caracteres | Validation_MaxLength |
| ImporteMinimo | GreaterThan(0) (When HasValue) | El importe minimo debe ser mayor a 0 | Validation_InvalidAmount |
| CantidadMaxima | GreaterThan(0) (When HasValue) | La cantidad maxima debe ser mayor a 0 | Validation_InvalidRange |
| CantidadPorBacker | GreaterThan(0) (When HasValue) | La cantidad por backer debe ser mayor a 0 | Validation_InvalidRange |
| TiempoEntregaEstimado | MaxLength(200) (When not null) | El tiempo de entrega estimado no puede superar los 200 caracteres | Validation_MaxLength |
| Orden | GreaterThanOrEqualTo(0) (When HasValue) | El orden debe ser mayor o igual a 0 | Validation_InvalidRange |

**Notas:**
- **NO modificar este archivo** - Ya tiene todas las validaciones necesarias
- La validacion de backings se hace en el **Handler**, NO en el validator

---

### 4.3 ReorderRewardsCommandValidator (NUEVO)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Validators/ReorderRewardsCommandValidator.cs`

**Estado:** ❌ NUEVO - A CREAR

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| CampaniaId | NotEmpty | El CampaniaId es obligatorio | Validation_Required |
| RewardOrders | NotNull | La lista de rewards es obligatoria | Validation_Required |
| RewardOrders | NotEmpty | La lista de rewards es obligatoria | Validation_Required |
| RewardOrders[].RewardId | NotEmpty | El RewardId es obligatorio | Validation_Required |
| RewardOrders[].Orden | GreaterThanOrEqualTo(0) | El orden debe ser mayor o igual a 0 | Validation_InvalidRange |

#### Contenido Completo del Archivo

```csharp
using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Rewards.Validators;

public class ReorderRewardsCommandValidator : AbstractValidator<ReorderRewardsCommand>
{
    public ReorderRewardsCommandValidator()
    {
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("El CampaniaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.RewardOrders)
            .NotNull()
            .WithMessage("La lista de rewards es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .NotEmpty()
            .WithMessage("La lista de rewards es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleForEach(x => x.RewardOrders)
            .ChildRules(order =>
            {
                order.RuleFor(x => x.RewardId)
                    .NotEmpty()
                    .WithMessage("El RewardId es obligatorio")
                    .WithErrorCode(ServiceResponseMessageType.Validation_Required);

                order.RuleFor(x => x.Orden)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("El orden debe ser mayor o igual a 0")
                    .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
            });
    }
}
```

**Notas:**
- Usa `RuleForEach()` para validar cada elemento de la lista `RewardOrders`
- Usa `ChildRules()` para validar propiedades del DTO `RewardOrderDto`
- NO inyectar services en validator (validaciones de negocio se hacen en Handler)

---

## 5. AutoMapper Profiles

### 5.1 RewardProfile (EXISTE - ACTUALIZAR)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/RewardProfile.cs`

**Estado:** ✅ YA EXISTE - Requiere agregar mapping para `UpdateRewardCommand`

**Mappings actuales (OK):**
- ✅ `CampaniaCrowdfundingReward` -> `RewardDto`
- ✅ `CampaniaCrowdfundingReward` -> `RewardListDto`
- ✅ `CreateRewardCommand` -> `CampaniaCrowdfundingReward`

**Mapping a AGREGAR (despues de linea 41):**

```csharp
// ---------------------------------------------------------------------
// UpdateCommand -> Entity (para actualizacion)
// ---------------------------------------------------------------------
CreateMap<UpdateRewardCommand, CampaniaCrowdfundingReward>()
    .ForMember(dest => dest.Id,
               opt => opt.MapFrom(src => new BuildingBlocks.EntityFramework.StronglyTypedIds.CampaniaCrowdfundingRewardId(src.Id)))
    .ForMember(dest => dest.CampaniaId, opt => opt.Ignore())
    .ForMember(dest => dest.EsActivo, opt => opt.MapFrom(src => src.EsActivo ?? default))
    .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
    .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom((src, dest) => DateTime.UtcNow))
    .ForMember(dest => dest.Campania, opt => opt.Ignore())
    .ForMember(dest => dest.Lineas, opt => opt.Ignore())
    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
```

**Notas:**
- El `.ForAllMembers(opts => opts.Condition(...))` asegura PATCH semantics (solo mapear propiedades NO nulas)
- **IMPORTANTE:** Actualmente el Handler de Update NO usa AutoMapper, hace mapping manual. Evaluar si migrar a AutoMapper para consistencia.

---

## 6. Services (Nuevos metodos requeridos)

### 6.1 IRewardService - Metodos a agregar

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Services/IRewardService.cs`

**Metodos NUEVOS a agregar:**

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetMaxOrdenAsync` | `Task<int>` | `CampaniaCrowdfundingId campaniaId, CancellationToken ct` | Obtiene el orden maximo de rewards de una campania (retorna 0 si no hay rewards) |
| `ReorderAsync` | `Task` | `CampaniaCrowdfundingId campaniaId, IEnumerable<(CampaniaCrowdfundingRewardId Id, int Orden)> updates, CancellationToken ct` | Reordena multiples rewards en bulk |
| `HasBackingsAsync` | `Task<bool>` | `CampaniaCrowdfundingRewardId id, CancellationToken ct` | Verifica si reward tiene backings asociados |

**Referencia:** Ver plan de Arquitectura Hexagonal (hexagonal-architecture.md) para implementacion completa de estos metodos.

---

## 7. Controller (Nuevo endpoint)

### 7.1 RewardsController - Endpoint a AGREGAR

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/RewardsController.cs`

**Endpoint NUEVO (agregar despues del metodo Delete, linea 166):**

```csharp
/// <summary>
/// Reorder rewards for a campaign.
/// </summary>
[HttpPut("reorder")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> Reorder(
    [FromBody] ReorderRewardsCommand command,
    CancellationToken cancellationToken)
{
    var result = await _mediator.Send(command, cancellationToken);

    if (!result.IsSuccess)
    {
        var errorCode = result.Messages.FirstOrDefault()?.ErrorCode;
        return errorCode switch
        {
            "2003" => NotFound(result),        // NotFound_Campania
            "2004" => NotFound(result),        // NotFound_Reward
            "3002" => StatusCode(403, result), // Auth_Forbidden
            "1001" => BadRequest(result),      // Validation_Required
            "1007" => BadRequest(result),      // Validation_InvalidRange
            _ => StatusCode(500, result)
        };
    }

    return Ok(result);
}
```

**Notas:**
- Endpoint autenticado con `[Authorize]`
- Retorna HTTP 403 si ArtistaId no coincide
- Retorna HTTP 404 si campania o rewards no existen
- Retorna HTTP 400 si validacion falla

---

## 8. Constants (Nueva constante)

### 8.1 ServiceResponseMessageType - Constante a AGREGAR

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Constants/ServiceResponseMessageType.cs`

**AGREGAR despues de linea 61 (despues de `BusinessRule_CampaniaNotDraft`):**

```csharp
public const string BusinessRule_RewardHasBackings = "4010";
```

**Contexto completo de la seccion Business Rule Errors (4000-4999):**

```csharp
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
public const string BusinessRule_RewardHasBackings = "4010";  // NUEVO
```

---

## 9. Archivos a Crear/Modificar

```
Modules/Crowdfunding/
├── WePlayRises.Crowdfunding.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs                 [MODIFICAR] - Agregar codigo 4010
│
├── WePlayRises.Crowdfunding.Application/
│   ├── Features/Rewards/
│   │   ├── Commands/
│   │   │   ├── CreateRewardCommand.cs                    [MODIFICAR] - Actualizar Handler (ownership, auto-orden)
│   │   │   ├── UpdateRewardCommand.cs                    [MODIFICAR] - Actualizar Handler (ownership, backings)
│   │   │   ├── DeleteRewardCommand.cs                    [REESCRIBIR] - Reescribir Handler completo
│   │   │   └── ReorderRewardsCommand.cs                  [CREAR] - Nuevo command + handler
│   │   ├── Queries/
│   │   │   ├── GetAllRewardsQuery.cs                     [VERIFICAR] - Agregar ordenamiento por Orden ASC
│   │   │   └── GetRewardByIdQuery.cs                     [OK] - Sin cambios
│   │   └── Validators/
│   │       ├── CreateRewardCommandValidator.cs           [OK] - Sin cambios
│   │       ├── UpdateRewardCommandValidator.cs           [OK] - Sin cambios
│   │       └── ReorderRewardsCommandValidator.cs         [CREAR] - Nuevo validator
│   ├── Interfaces/Services/
│   │   └── IRewardService.cs                             [MODIFICAR] - Agregar 3 metodos nuevos
│   └── Mapping/
│       └── RewardProfile.cs                              [MODIFICAR] - Agregar mapping UpdateCommand -> Entity
│
├── WePlayRises.Crowdfunding.Infra/
│   ├── Services/
│   │   └── RewardService.cs                              [MODIFICAR] - Implementar 3 metodos nuevos
│   └── Repositories/
│       └── RewardRepository.cs                           [MODIFICAR] - Implementar 4 metodos nuevos
│
└── WePlayRises.Crowdfunding.WebApi/
    └── Controllers/
        └── RewardsController.cs                          [MODIFICAR] - Agregar endpoint PUT /reorder
```

---

## 10. Resumen de Acciones

| Archivo | Accion | Prioridad | Detalles |
|---------|--------|-----------|----------|
| `ServiceResponseMessageType.cs` | **MODIFICAR** | ALTA | Agregar codigo 4010 en linea 62 |
| `ReorderRewardsCommand.cs` | **CREAR** | ALTA | Nuevo archivo completo (DTO + Command + Handler) |
| `ReorderRewardsCommandValidator.cs` | **CREAR** | ALTA | Nuevo archivo completo (Validator) |
| `CreateRewardCommand.cs` | **MODIFICAR** | ALTA | Actualizar Handler: ownership + auto-orden |
| `UpdateRewardCommand.cs` | **MODIFICAR** | ALTA | Actualizar Handler: ownership + backings |
| `DeleteRewardCommand.cs` | **REESCRIBIR** | ALTA | Reescribir Handler completo con validaciones |
| `RewardsController.cs` | **MODIFICAR** | MEDIA | Agregar endpoint PUT /reorder (linea 166) |
| `RewardProfile.cs` | **MODIFICAR** | MEDIA | Agregar mapping UpdateCommand (opcional) |
| `GetAllRewardsQuery.cs` | **VERIFICAR** | MEDIA | Verificar ordenamiento por Orden ASC |
| `IRewardService.cs` | **MODIFICAR** | ALTA | Agregar 3 metodos nuevos (ver hexagonal-architecture.md) |
| `RewardService.cs` | **MODIFICAR** | ALTA | Implementar 3 metodos nuevos (ver hexagonal-architecture.md) |
| `IRewardRepository.cs` | **MODIFICAR** | ALTA | Agregar 4 metodos nuevos (ver hexagonal-architecture.md) |
| `RewardRepository.cs` | **MODIFICAR** | ALTA | Implementar 4 metodos nuevos (ver hexagonal-architecture.md) |

---

## 11. Patrones Importantes

### 11.1 Logica de Negocio en Handler

```
Handler contiene:
- Validacion (FluentValidation)
- Validacion de ownership (ArtistaId del token)
- Validacion de backings (antes de modificar/eliminar)
- Logica de negocio (auto-incrementar orden, soft delete, etc.)
- Orquestacion de llamadas a services

Service contiene:
- SOLO persistencia (CRUD via repository)
- Metodos de consulta (GetMaxOrdenAsync, HasBackingsAsync)
- NO logica de negocio
```

### 11.2 ServiceResponse siempre con Constants

```csharp
// Exito - USAR CONSTANTS
return new ServiceResponse<T>
{
    Data = result,
    Messages = new List<ServiceResponseMessage>
    {
        new() { Message = "Creado", ErrorCode = ServiceResponseMessageType.Created }
    }
};

// Error de validacion (FluentValidation maneja el ErrorCode automaticamente)
return new ServiceResponse<T> { Messages = validationErrors };

// Error de negocio - USAR CONSTANTS
return new ServiceResponse<T>
{
    Messages = new List<ServiceResponseMessage>
    {
        new() { Message = "...", ErrorCode = ServiceResponseMessageType.BusinessRule_RewardHasBackings }
    }
};

// Error de autorizacion - USAR CONSTANTS
return new ServiceResponse<T>
{
    Messages = new List<ServiceResponseMessage>
    {
        new() { Message = "...", ErrorCode = ServiceResponseMessageType.Auth_Forbidden }
    }
};
```

### 11.3 Auto-incrementar Orden

```csharp
// En CreateRewardCommandHandler, despues de mapear entity:
if (entity.Orden == 0)
{
    var maxOrden = await _rewardService.GetMaxOrdenAsync(campaniaId, cancellationToken);
    entity.Orden = maxOrden + 1;
}
```

### 11.4 Soft Delete con Validacion de Backings

```csharp
// En DeleteRewardCommandHandler:

// 1. Validar que NO tiene backings
var hasBackings = await _rewardService.HasBackingsAsync(rewardId, cancellationToken);
if (hasBackings)
{
    return new ServiceResponse<bool>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "No se puede eliminar...", ErrorCode = ServiceResponseMessageType.BusinessRule_RewardHasBackings }
        }
    };
}

// 2. Soft delete (NO usar DeleteAsync)
entity.EsActivo = false;
entity.FechaActualizacion = DateTime.UtcNow;
await _rewardService.UpdateAsync(entity, cancellationToken);
```

### 11.5 Validacion de Ownership (Pendiente implementacion completa)

```csharp
// TODO: Implementar extraccion de ArtistaId desde JWT claims via IHttpContextAccessor
// Este patron se repetira en todos los Handlers de Commands autenticados

// Pseudocodigo:
// var artistaIdFromToken = GetArtistaIdFromToken(); // Desde JWT claims
// var campania = await _campaniaService.GetByIdAsync(campaniaId, cancellationToken);
// if (campania.ArtistaId.Value != artistaIdFromToken)
// {
//     return new ServiceResponse<T>
//     {
//         Messages = new List<ServiceResponseMessage>
//         {
//             new() { Message = "No tienes permiso...", ErrorCode = ServiceResponseMessageType.Auth_Forbidden }
//         }
//     };
// }
```

**NOTA CRITICA:** La implementacion completa de validacion de ownership requiere:
1. Inyectar `IHttpContextAccessor` en los Handlers
2. Crear metodo helper `GetArtistaIdFromToken()` que extraiga el claim `sub` del JWT
3. Llamar a `_campaniaService.GetByIdAsync()` para obtener la campania y su ArtistaId
4. Comparar ArtistaId de la campania con ArtistaId del token

---

## 12. Checklist

- [ ] Codigo 4010 agregado en `ServiceResponseMessageType.cs`
- [ ] `ReorderRewardsCommand.cs` creado con Handler
- [ ] `ReorderRewardsCommandValidator.cs` creado
- [ ] `CreateRewardCommand.cs` Handler actualizado (ownership + auto-orden)
- [ ] `UpdateRewardCommand.cs` Handler actualizado (ownership + backings)
- [ ] `DeleteRewardCommand.cs` Handler reescrito completo
- [ ] `RewardsController.cs` actualizado con endpoint PUT /reorder
- [ ] `RewardProfile.cs` actualizado con mapping UpdateCommand (opcional)
- [ ] `GetAllRewardsQuery.cs` verificado ordenamiento por Orden ASC
- [ ] `IRewardService` actualizado con 3 metodos nuevos
- [ ] `RewardService` implementa 3 metodos nuevos
- [ ] `IRewardRepository` actualizado con 4 metodos nuevos
- [ ] `RewardRepository` implementa 4 metodos nuevos
- [ ] Handler + Command en MISMO archivo (`ReorderRewardsCommand.cs`)
- [ ] Retorna `ServiceResponse<T>` en todos los handlers
- [ ] Constructor con `?? throw new ArgumentNullException` para TODAS las dependencias
- [ ] Validators usan `ServiceResponseMessageType.X` constants (NO strings literales)
- [ ] Handlers usan `ServiceResponseMessageType.X` en respuestas
- [ ] Logger inyectado y usado en try-catch de todos los handlers
- [ ] Services inyectados (NO DbContext) en handlers
- [ ] Validacion retorna ServiceResponse (NO throw exceptions)

---

## 13. Proximos Pasos Sugeridos

Una vez implementado este plan CQRS:

1. **Backend Implementation Agent** - Implementar los archivos planificados
   - Crear `ReorderRewardsCommand.cs` y `ReorderRewardsCommandValidator.cs`
   - Modificar handlers de Create, Update, Delete
   - Actualizar Controller con nuevo endpoint
   - Agregar constante 4010 en Constants
   - Implementar metodos faltantes en Services/Repositories (ver hexagonal-architecture.md)

2. **Testing** - Crear unit tests
   - `ReorderRewardsCommandHandlerTests`
   - `ReorderRewardsCommandValidatorTests`
   - Tests de validacion de ownership
   - Tests de validacion de backings en Update/Delete

3. **Admin Dashboard** - Implementar UI de gestion
   - Lista de recompensas con drag & drop
   - Formulario modal de creacion/edicion
   - Validacion de ownership en frontend

4. **Landing** - Vista publica de recompensas
   - Lista de recompensas en detalle de campania
   - Badge "Agotado" para stock 0

---

**Puntos clave de este plan CQRS:**

- ✅ Handler + Command en MISMO archivo para todos los Commands/Queries
- ✅ SIEMPRE retornar `ServiceResponse<T>`
- ✅ Handler NUNCA inyecta DbContext (usa Services)
- ✅ Services retornan Entidades, NO DTOs
- ✅ Validators con `.WithMessage()` Y `.WithErrorCode(ServiceResponseMessageType.X)`
- ✅ Validators en carpeta `Validators/` separada
- ✅ Logica de negocio en Handlers, NO en Services
- ✅ SIEMPRE `?? throw new ArgumentNullException` en constructores
- ✅ SIEMPRE usar `ServiceResponseMessageType` constants (NO strings literales)
- ✅ Try-catch con logging en todos los Handlers
- ✅ Validacion de ownership en Handlers (ArtistaId del token vs ArtistaId de la campania)
- ✅ Validacion de backings antes de eliminar/modificar (retornar error 4010 si tiene backings)

---

**Fin del plan CQRS.**
