# Contratos API: Definir Recompensas

**Fecha:** 2026-02-13
**Modulo:** Crowdfunding
**Feature:** definir-recompensas
**Basado en:** docs/user-stories/definir-recompensas/contracts.md

---

## 1. Endpoints

| Metodo | Ruta | Tipo | Descripcion | Auth | Estado |
|--------|------|------|-------------|------|--------|
| POST | /api/rewards | Command | Crear recompensa | ✅ Bearer | ✅ EXISTE |
| GET | /api/rewards | Query | Listar con filtros | ❌ Publica | ✅ EXISTE |
| GET | /api/rewards/{id} | Query | Obtener por ID | ❌ Publica | ✅ EXISTE |
| PUT | /api/rewards/{id} | Command | Actualizar recompensa | ✅ Bearer | ✅ EXISTE |
| DELETE | /api/rewards/{id} | Command | Eliminar (soft delete) | ✅ Bearer | ✅ EXISTE |
| PUT | /api/rewards/reorder | Command | Reordenar recompensas | ✅ Bearer | ❌ NUEVO |

---

## 2. Request DTOs

### 2.1 CreateRewardCommand (EXISTE - REVISAR)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Commands/CreateRewardCommand.cs`

**Estado:** ✅ **YA EXISTE** - Requiere validacion de ownership

| Propiedad | Tipo | Requerido | Default | Descripcion |
|-----------|------|-----------|---------|-------------|
| CampaniaId | Guid | Si | - | ID de la campania |
| TipoRewardId | int | Si | - | Tipo de recompensa (1=Digital, 2=Fisico, 3=Experiencia, 4=Otro) |
| Nombre | string | Si | - | Nombre de la recompensa (max 200) |
| Descripcion | string? | No | null | Descripcion detallada (max 2000) |
| ImporteMinimo | decimal | Si | - | Monto minimo de aportacion (> 0) |
| MonedaId | int | Si | 1 | Moneda (1=EUR por MVP) |
| EsAddOn | bool | Si | false | Si es complemento opcional |
| CantidadMaxima | int? | No | null | Stock limitado (null = ilimitado) |
| CantidadPorBacker | int? | No | null | Limite por backer |
| IncluyeEnvioFisico | bool | Si | false | Requiere direccion de envio |
| TiempoEntregaEstimado | string? | No | null | Texto libre (max 200) |
| Orden | int | Si | 0 | Orden de visualizacion (>= 0) |

**Implementa:** `IRequest<ServiceResponse<RewardDto>>`

**Notas:**
- El campo `EsActivo` se setea automaticamente a `true` en el Handler
- El campo `Orden` debe auto-incrementarse si no se proporciona (max + 1)

**Cambios requeridos:**
- ❌ **NO hay cambios en el Command** - La estructura es correcta
- ✅ **AGREGAR validacion de ownership en CreateRewardCommandValidator** (ver seccion 4.1)

---

### 2.2 UpdateRewardCommand (EXISTE - OK)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Commands/UpdateRewardCommand.cs`

**Estado:** ✅ **YA EXISTE** - Validacion OK, requiere ownership check

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

**Notas:**
- Todos los campos son opcionales excepto `Id`
- Solo se actualizan los campos proporcionados (PATCH semantics)

**Cambios requeridos:**
- ✅ **AGREGAR validacion de ownership en UpdateRewardCommandValidator** (ver seccion 4.2)
- ✅ **AGREGAR validacion de backings antes de modificar campos criticos** (ImporteMinimo, CantidadMaxima)

---

### 2.3 DeleteRewardCommand (EXISTE - REVISAR)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Commands/DeleteRewardCommand.cs`

**Estado:** ✅ **YA EXISTE** - Requiere validacion de backings

| Propiedad | Tipo | Requerido |
|-----------|------|-----------|
| Id | Guid | Si |

**Implementa:** `IRequest<ServiceResponse<bool>>`

**Notas:**
- Usa soft delete (EsActivo = false)
- NO permite eliminar si tiene backings asociados

**Cambios requeridos:**
- ✅ **AGREGAR validacion de backings en DeleteRewardCommandHandler**
- ✅ **AGREGAR validacion de ownership**

---

### 2.4 ReorderRewardsCommand (NUEVO)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Commands/ReorderRewardsCommand.cs`

**Estado:** ❌ **NUEVO - A CREAR**

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| CampaniaId | Guid | Si | ID de la campania |
| RewardOrders | List\<RewardOrderDto\> | Si | Lista de reordenamientos |

**Implementa:** `IRequest<ServiceResponse<bool>>`

**RewardOrderDto:**

| Propiedad | Tipo | Requerido |
|-----------|------|-----------|
| RewardId | Guid | Si |
| Orden | int | Si (>= 0) |

**Contenido completo del archivo:**

```csharp
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
    private readonly ILogger<ReorderRewardsCommandHandler> _logger;

    public ReorderRewardsCommandHandler(
        IRewardService rewardService,
        ICampaniaService campaniaService,
        ILogger<ReorderRewardsCommandHandler> logger)
    {
        _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<bool>> Handle(
        ReorderRewardsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar que la campania existe
            var campaniaId = new CampaniaCrowdfundingId(request.CampaniaId);
            var campaniaExists = await _campaniaService.ExistsAsync(campaniaId, cancellationToken);
            if (!campaniaExists)
            {
                return ValidateExtensions.NotFoundServiceResponse<bool>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            // 2. Validar ownership (ArtistaId del token debe coincidir con ArtistaId de la campania)
            // TODO: Implementar extraccion de ArtistaId desde JWT claims
            // var artistaIdFromToken = GetArtistaIdFromToken();
            // var campania = await _campaniaService.GetByIdAsync(campaniaId, cancellationToken);
            // if (campania.ArtistaId != artistaIdFromToken)
            // {
            //     return ValidateExtensions.ForbiddenServiceResponse<bool>(
            //         "No tienes permiso para reordenar recompensas en esta campania",
            //         ServiceResponseMessageType.Auth_Forbidden);
            // }

            // 3. Validar lista de rewards
            if (request.RewardOrders == null || !request.RewardOrders.Any())
            {
                return ValidateExtensions.ValidationErrorServiceResponse<bool>(
                    "La lista de rewards es obligatoria",
                    ServiceResponseMessageType.Validation_Required);
            }

            // 4. Validar ordenes
            if (request.RewardOrders.Any(r => r.Orden < 0))
            {
                return ValidateExtensions.ValidationErrorServiceResponse<bool>(
                    "Todos los ordenes deben ser mayor o igual a 0",
                    ServiceResponseMessageType.Validation_InvalidRange);
            }

            // 5. Actualizar ordenes
            foreach (var rewardOrder in request.RewardOrders)
            {
                var rewardId = new CampaniaCrowdfundingRewardId(rewardOrder.RewardId);
                var reward = await _rewardService.GetByIdAsync(rewardId, cancellationToken);

                if (reward == null)
                {
                    return ValidateExtensions.NotFoundServiceResponse<bool>(
                        "Uno o mas rewards no encontrados",
                        ServiceResponseMessageType.NotFound_Reward);
                }

                // Validar que el reward pertenece a la campania
                if (reward.CampaniaId.Value != request.CampaniaId)
                {
                    return ValidateExtensions.ValidationErrorServiceResponse<bool>(
                        "Uno o mas rewards no pertenecen a la campania especificada",
                        ServiceResponseMessageType.BusinessRule_OperationNotAllowed);
                }

                // Actualizar orden
                reward.Orden = rewardOrder.Orden;
                await _rewardService.UpdateAsync(reward, cancellationToken);
            }

            _logger.LogInformation("Rewards reordenados exitosamente para Campania {CampaniaId}", request.CampaniaId);

            return new ServiceResponse<bool>
            {
                Data = true,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Recompensas reordenadas correctamente",
                        ErrorCode = ServiceResponseMessageType.Updated
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

---

### 2.5 GetAllRewardsQuery (EXISTE - OK)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Queries/GetAllRewardsQuery.cs`

**Estado:** ✅ **YA EXISTE** - Sin cambios requeridos

| Query Param | Tipo | Requerido | Descripcion |
|-------------|------|-----------|-------------|
| campaniaId | Guid? | No | Filtrar por campania |
| esActivo | bool? | No | Filtrar por estado activo/inactivo |
| esAddOn | bool? | No | Filtrar por tipo add-on |

**Implementa:** `IRequest<ServiceResponse<IEnumerable<RewardListDto>>>`

---

### 2.6 GetRewardByIdQuery (EXISTE - OK)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Queries/GetRewardByIdQuery.cs`

**Estado:** ✅ **YA EXISTE** - Sin cambios requeridos

| Propiedad | Tipo | Requerido |
|-----------|------|-----------|
| Id | Guid | Si |

**Implementa:** `IRequest<ServiceResponse<RewardDto>>`

---

## 3. Response DTOs

### 3.1 RewardDto (EXISTE - OK)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/RewardDto.cs`

**Estado:** ✅ **YA EXISTE** - Estructura correcta

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador unico |
| CampaniaId | Guid | ID de la campania |
| TipoRewardId | int | Tipo de recompensa |
| Nombre | string | Nombre de la recompensa |
| Descripcion | string? | Descripcion detallada |
| ImporteMinimo | decimal | Importe minimo requerido |
| MonedaId | int | Moneda (1=EUR) |
| EsAddOn | bool | Es complemento opcional |
| CantidadMaxima | int? | Stock limitado (null = ilimitado) |
| CantidadPorBacker | int? | Limite por backer |
| IncluyeEnvioFisico | bool | Requiere direccion de envio |
| TiempoEntregaEstimado | string? | Texto libre de entrega |
| Orden | int | Orden de visualizacion |
| EsActivo | bool | Estado activo/inactivo |
| FechaCreacion | DateTime | Fecha de creacion |
| FechaActualizacion | DateTime? | Fecha de ultima actualizacion |

**Usado en:**
- POST /api/rewards → Response 200 OK
- GET /api/rewards/{id} → Response 200 OK

---

### 3.2 RewardListDto (EXISTE - OK)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/RewardListDto.cs`

**Estado:** ✅ **YA EXISTE** - Estructura correcta

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador unico |
| CampaniaId | Guid | ID de la campania |
| Nombre | string | Nombre de la recompensa |
| Descripcion | string? | Descripcion detallada |
| ImporteMinimo | decimal | Importe minimo requerido |
| EsAddOn | bool | Es complemento opcional |
| CantidadMaxima | int? | Stock limitado |
| IncluyeEnvioFisico | bool | Requiere envio |
| Orden | int | Orden de visualizacion |
| EsActivo | bool | Estado activo/inactivo |

**Usado en:**
- GET /api/rewards → Response 200 OK (lista)

---

## 4. Validadores

### 4.1 CreateRewardCommandValidator (EXISTE - ACTUALIZAR)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Validators/CreateRewardCommandValidator.cs`

**Estado:** ✅ **EXISTE** - Requiere agregar validacion de ownership

**Validaciones actuales (OK):**

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

**Cambios requeridos:**

❌ **NO agregar validaciones adicionales en el validator**. La validacion de ownership se debe hacer en el **Handler**, NO en el validator.

**Razon:** El validator NO debe inyectar services para validacion de ownership. Esto se hace en el Handler para mantener separacion de responsabilidades.

**IMPORTANTE:** El archivo actual ya tiene todas las validaciones necesarias. **NO modificar este archivo.**

---

### 4.2 UpdateRewardCommandValidator (EXISTE - ACTUALIZAR)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Validators/UpdateRewardCommandValidator.cs`

**Estado:** ✅ **EXISTE** - Requiere agregar validacion de backings (en Handler, NO validator)

**Validaciones actuales (OK):**

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

**Cambios requeridos:**

❌ **NO agregar validaciones adicionales en el validator**. La validacion de backings y ownership se debe hacer en el **Handler**.

**IMPORTANTE:** El archivo actual ya tiene todas las validaciones necesarias. **NO modificar este archivo.**

---

### 4.3 ReorderRewardsCommandValidator (NUEVO)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Validators/ReorderRewardsCommandValidator.cs`

**Estado:** ❌ **NUEVO - A CREAR**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| CampaniaId | NotEmpty | El CampaniaId es obligatorio | Validation_Required |
| RewardOrders | NotNull | La lista de rewards es obligatoria | Validation_Required |
| RewardOrders | NotEmpty | La lista de rewards es obligatoria | Validation_Required |
| RewardOrders[].RewardId | NotEmpty | El RewardId es obligatorio | Validation_Required |
| RewardOrders[].Orden | GreaterThanOrEqualTo(0) | El orden debe ser mayor o igual a 0 | Validation_InvalidRange |

**Contenido completo del archivo:**

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

---

## 5. AutoMapper Mappings

### 5.1 RewardProfile (EXISTE - ACTUALIZAR)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/RewardProfile.cs`

**Estado:** ✅ **EXISTE** - Requiere agregar mapping para UpdateRewardCommand

**Mappings actuales (OK):**

| Source | Destination | Notas |
|--------|-------------|-------|
| CampaniaCrowdfundingReward | RewardDto | Con StronglyTypedId unwrapping |
| CampaniaCrowdfundingReward | RewardListDto | Con StronglyTypedId unwrapping |
| CreateRewardCommand | CampaniaCrowdfundingReward | Con StronglyTypedId wrapping, ignore Id/EsActivo/Fechas |

**Cambios requeridos:**

Agregar mapping para `UpdateRewardCommand`:

```csharp
// Agregar despues del mapping de CreateRewardCommand (linea 41):

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

**Nota:** El `.ForAllMembers(opts => opts.Condition(...))` asegura que solo se mapean propiedades NO nulas (PATCH semantics).

---

## 6. OpenAPI / Swagger Documentation

### 6.1 POST /api/rewards

**Summary:** Crear nueva recompensa para una campania

**Request Body:** CreateRewardCommand

**Responses:**

| Status | Type | Descripcion | ErrorCode |
|--------|------|-------------|-----------|
| 200 | ServiceResponse\<RewardDto\> | Recompensa creada exitosamente | 0000 |
| 400 | ServiceResponse\<RewardDto\> | Validacion fallida | 1001, 1002, 1007, 1011 |
| 401 | ServiceResponse\<RewardDto\> | Token JWT invalido/expirado | 3001 |
| 403 | ServiceResponse\<RewardDto\> | No autorizado (ArtistaId mismatch) | 3002 |
| 404 | ServiceResponse\<RewardDto\> | Campania no encontrada | 2003 |
| 500 | ServiceResponse\<RewardDto\> | Error inesperado | 5000 |

**Auth:** Bearer token requerido

**Claims JWT requeridos:**
- `sub`: ArtistaId (Guid) - debe coincidir con `campania.ArtistaId`

---

### 6.2 GET /api/rewards

**Summary:** Listar recompensas con filtros opcionales

**Query Parameters:**
- `campaniaId` (Guid, opcional): Filtra por campania
- `esActivo` (bool, opcional): Filtra por estado activo/inactivo
- `esAddOn` (bool, opcional): Filtra por tipo add-on

**Responses:**

| Status | Type | Descripcion | ErrorCode |
|--------|------|-------------|-----------|
| 200 | ServiceResponse\<IEnumerable\<RewardListDto\>\> | Lista de recompensas | 0000 |
| 500 | ServiceResponse\<IEnumerable\<RewardListDto\>\> | Error inesperado | 5000 |

**Auth:** Publica (no requiere autenticacion)

---

### 6.3 GET /api/rewards/{id}

**Summary:** Obtener detalle de una recompensa por ID

**Path Parameters:**
- `id` (Guid, requerido): ID de la recompensa

**Responses:**

| Status | Type | Descripcion | ErrorCode |
|--------|------|-------------|-----------|
| 200 | ServiceResponse\<RewardDto\> | Recompensa encontrada | 0000 |
| 404 | ServiceResponse\<RewardDto\> | Recompensa no encontrada | 2004 |
| 500 | ServiceResponse\<RewardDto\> | Error inesperado | 5000 |

**Auth:** Publica (no requiere autenticacion)

---

### 6.4 PUT /api/rewards/{id}

**Summary:** Actualizar una recompensa existente

**Path Parameters:**
- `id` (Guid, requerido): ID de la recompensa (debe coincidir con body)

**Request Body:** UpdateRewardCommand

**Responses:**

| Status | Type | Descripcion | ErrorCode |
|--------|------|-------------|-----------|
| 200 | ServiceResponse\<bool\> | Recompensa actualizada | 0002 |
| 400 | ServiceResponse\<bool\> | Validacion fallida o ID mismatch | 1001, 1002, 1007, 1011 |
| 401 | ServiceResponse\<bool\> | Token JWT invalido/expirado | 3001 |
| 403 | ServiceResponse\<bool\> | No autorizado (ArtistaId mismatch) | 3002 |
| 404 | ServiceResponse\<bool\> | Recompensa no encontrada | 2004 |
| 409 | ServiceResponse\<bool\> | No se puede modificar (tiene backings) | 4010 |
| 500 | ServiceResponse\<bool\> | Error inesperado | 5000 |

**Auth:** Bearer token requerido

**Notas:**
- Si la recompensa tiene backings asociados, solo se permite modificar `Descripcion`, `TiempoEntregaEstimado` y `EsActivo`
- Cambios en `ImporteMinimo` o `CantidadMaxima` con backings retornan error 4010

---

### 6.5 DELETE /api/rewards/{id}

**Summary:** Eliminar (desactivar) una recompensa

**Path Parameters:**
- `id` (Guid, requerido): ID de la recompensa

**Responses:**

| Status | Type | Descripcion | ErrorCode |
|--------|------|-------------|-----------|
| 200 | ServiceResponse\<bool\> | Recompensa eliminada (soft delete) | 0003 |
| 401 | ServiceResponse\<bool\> | Token JWT invalido/expirado | 3001 |
| 403 | ServiceResponse\<bool\> | No autorizado (ArtistaId mismatch) | 3002 |
| 404 | ServiceResponse\<bool\> | Recompensa no encontrada | 2004 |
| 409 | ServiceResponse\<bool\> | No se puede eliminar (tiene backings) | 4010 |
| 500 | ServiceResponse\<bool\> | Error inesperado | 5000 |

**Auth:** Bearer token requerido

**Notas:**
- Soft delete: pone `EsActivo = false`
- Si tiene backings asociados, retorna error 4010 y NO permite eliminar

---

### 6.6 PUT /api/rewards/reorder (NUEVO)

**Summary:** Reordenar multiples recompensas de una campania

**Request Body:** ReorderRewardsCommand

**Responses:**

| Status | Type | Descripcion | ErrorCode |
|--------|------|-------------|-----------|
| 200 | ServiceResponse\<bool\> | Recompensas reordenadas | 0002 |
| 400 | ServiceResponse\<bool\> | Validacion fallida | 1001, 1007 |
| 401 | ServiceResponse\<bool\> | Token JWT invalido/expirado | 3001 |
| 403 | ServiceResponse\<bool\> | No autorizado (ArtistaId mismatch) | 3002 |
| 404 | ServiceResponse\<bool\> | Campania o rewards no encontrados | 2003, 2004 |
| 500 | ServiceResponse\<bool\> | Error inesperado | 5000 |

**Auth:** Bearer token requerido

**Notas:**
- Actualiza el campo `Orden` de multiples rewards en una sola transaccion
- Todos los rewards deben pertenecer a la campania especificada

---

## 7. Actualizacion de ServiceResponseMessageType Constants

### 7.1 Nueva Constante Requerida

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Constants/ServiceResponseMessageType.cs`

**Estado:** ✅ **EXISTE** - Requiere agregar codigo 4010

**Agregar en seccion "BUSINESS RULE ERRORS (4000-4999)" (despues de linea 61):**

```csharp
public const string BusinessRule_RewardHasBackings = "4010";
```

**Contenido completo de la seccion actualizada:**

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

**Uso:**
- `UpdateRewardCommandHandler`: Validar backings antes de modificar campos criticos
- `DeleteRewardCommandHandler`: Validar backings antes de soft delete

---

## 8. Controller Endpoints

### 8.1 RewardsController (EXISTE - ACTUALIZAR)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/RewardsController.cs`

**Estado:** ✅ **EXISTE** - Requiere agregar endpoint de reordenamiento

**Endpoints actuales (OK):**
- ✅ GET /api/rewards (GetAll)
- ✅ GET /api/rewards/{id} (GetById)
- ✅ POST /api/rewards (Create)
- ✅ PUT /api/rewards/{id} (Update)
- ✅ DELETE /api/rewards/{id} (Delete)

**Cambios requeridos:**

Agregar endpoint para reordenar recompensas (despues del metodo Delete, linea 166):

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

---

## 9. Archivos a Crear/Modificar

```
Modules/Crowdfunding/
├── WePlayRises.Crowdfunding.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs          [MODIFICAR] - Agregar codigo 4010
│
├── WePlayRises.Crowdfunding.Application/
│   ├── Features/Rewards/
│   │   ├── Commands/
│   │   │   ├── CreateRewardCommand.cs             [OK] - Sin cambios
│   │   │   ├── UpdateRewardCommand.cs             [OK] - Sin cambios
│   │   │   ├── DeleteRewardCommand.cs             [MODIFICAR] - Agregar validacion backings en Handler
│   │   │   └── ReorderRewardsCommand.cs           [CREAR] - Nuevo command + handler
│   │   ├── Queries/
│   │   │   ├── GetAllRewardsQuery.cs              [OK] - Sin cambios
│   │   │   └── GetRewardByIdQuery.cs              [OK] - Sin cambios
│   │   └── Validators/
│   │       ├── CreateRewardCommandValidator.cs    [OK] - Sin cambios
│   │       ├── UpdateRewardCommandValidator.cs    [OK] - Sin cambios
│   │       └── ReorderRewardsCommandValidator.cs  [CREAR] - Nuevo validator
│   ├── Dtos/
│   │   ├── RewardDto.cs                           [OK] - Sin cambios
│   │   └── RewardListDto.cs                       [OK] - Sin cambios
│   └── Mapping/
│       └── RewardProfile.cs                       [MODIFICAR] - Agregar mapping UpdateCommand -> Entity
│
└── WePlayRises.Crowdfunding.WebApi/
    └── Controllers/
        └── RewardsController.cs                   [MODIFICAR] - Agregar endpoint PUT /reorder
```

### Resumen de Acciones:

| Archivo | Accion | Prioridad | Razon |
|---------|--------|-----------|-------|
| `ServiceResponseMessageType.cs` | **MODIFICAR** | ALTA | Agregar codigo 4010 para business rule |
| `ReorderRewardsCommand.cs` | **CREAR** | ALTA | Nuevo command + handler para reordenamiento |
| `ReorderRewardsCommandValidator.cs` | **CREAR** | ALTA | Validator para nuevo command |
| `RewardProfile.cs` | **MODIFICAR** | MEDIA | Agregar mapping UpdateCommand -> Entity |
| `RewardsController.cs` | **MODIFICAR** | MEDIA | Agregar endpoint PUT /reorder |
| `DeleteRewardCommand.cs` | **MODIFICAR** | MEDIA | Agregar validacion de backings en Handler |
| `CreateRewardCommand.cs` | **OK** | - | Sin cambios (validacion ownership en Handler) |
| `UpdateRewardCommand.cs` | **OK** | - | Sin cambios (validacion ownership en Handler) |
| `CreateRewardCommandValidator.cs` | **OK** | - | Sin cambios (validaciones completas) |
| `UpdateRewardCommandValidator.cs` | **OK** | - | Sin cambios (validaciones completas) |

---

## 10. Validacion de Ownership (Autorizacion)

### 10.1 Flujo de Validacion

Para todos los endpoints autenticados (POST, PUT, DELETE, PUT/reorder):

```csharp
// Pseudocodigo del flujo en Handler:

// 1. Extraer ArtistaId del token JWT
var artistaIdClaim = User.FindFirst("sub")?.Value;
if (string.IsNullOrEmpty(artistaIdClaim) || !Guid.TryParse(artistaIdClaim, out var artistaIdFromToken))
{
    return ValidateExtensions.UnauthorizedServiceResponse<T>(
        "Token no valido",
        ServiceResponseMessageType.Auth_Unauthorized);
}

// 2. Obtener la campania asociada
var campania = await _campaniaService.GetByIdAsync(campaniaId, cancellationToken);
if (campania == null)
{
    return ValidateExtensions.NotFoundServiceResponse<T>(
        "Campania no encontrada",
        ServiceResponseMessageType.NotFound_Campania);
}

// 3. Validar ownership
if (campania.ArtistaId.Value != artistaIdFromToken)
{
    return ValidateExtensions.ForbiddenServiceResponse<T>(
        "No tienes permiso para modificar esta recompensa",
        ServiceResponseMessageType.Auth_Forbidden);
}

// 4. Continuar con la logica del Handler...
```

### 10.2 Donde Implementar

- ✅ **CreateRewardCommandHandler** - Validar ownership despues de verificar que la campania existe (linea 86)
- ✅ **UpdateRewardCommandHandler** - Validar ownership antes de actualizar
- ✅ **DeleteRewardCommandHandler** - Validar ownership antes de eliminar
- ✅ **ReorderRewardsCommandHandler** - Validar ownership despues de verificar que la campania existe (comentado en codigo de ejemplo)

**IMPORTANTE:** La validacion de ownership se hace en el **Handler**, NO en el validator. El validator NO debe inyectar services.

---

## 11. Validacion de Backings

### 11.1 Flujo de Validacion

Para `UpdateRewardCommand` y `DeleteRewardCommand`:

```csharp
// En el Handler, despues de obtener la entidad reward:

// Verificar si tiene backings asociados
var hasBackings = await _rewardService.HasBackingsAsync(rewardId, cancellationToken);

// Para DELETE: Siempre bloquear si tiene backings
if (hasBackings)
{
    return ValidateExtensions.ConflictServiceResponse<bool>(
        "No se puede eliminar una recompensa con aportes existentes",
        ServiceResponseMessageType.BusinessRule_RewardHasBackings);
}

// Para UPDATE: Bloquear cambios en campos criticos si tiene backings
if (hasBackings && (request.ImporteMinimo.HasValue || request.CantidadMaxima.HasValue))
{
    return ValidateExtensions.ConflictServiceResponse<bool>(
        "No se puede modificar el importe o cantidad maxima de una recompensa con aportes existentes",
        ServiceResponseMessageType.BusinessRule_RewardHasBackings);
}
```

### 11.2 Metodo de Service Requerido

**Agregar en `IRewardService` y `RewardService`:**

```csharp
// Interface
Task<bool> HasBackingsAsync(CampaniaCrowdfundingRewardId rewardId, CancellationToken cancellationToken);

// Implementacion
public async Task<bool> HasBackingsAsync(CampaniaCrowdfundingRewardId rewardId, CancellationToken cancellationToken)
{
    var reward = await _repository.GetByIdAsync(rewardId, cancellationToken);
    return reward?.Lineas?.Any() ?? false;
}
```

**Nota:** `Lineas` es la navigation property de `CampaniaCrowdfundingReward` a `CampaniaCrowdfundingPedidoLinea` (backings).

---

## 12. Checklist de Implementacion

- [ ] Codigo 4010 agregado en `ServiceResponseMessageType.cs`
- [ ] `ReorderRewardsCommand.cs` creado con Handler
- [ ] `ReorderRewardsCommandValidator.cs` creado
- [ ] `RewardProfile.cs` actualizado con mapping UpdateCommand -> Entity
- [ ] `RewardsController.cs` actualizado con endpoint PUT /reorder
- [ ] `DeleteRewardCommandHandler` actualizado con validacion de backings
- [ ] `CreateRewardCommandHandler` actualizado con validacion de ownership
- [ ] `UpdateRewardCommandHandler` actualizado con validacion de ownership y backings
- [ ] `IRewardService` y `RewardService` actualizados con metodo `HasBackingsAsync()`
- [ ] Handler + Command en MISMO archivo (ReorderRewardsCommand.cs)
- [ ] Retorna ServiceResponse\<T\> en todos los handlers
- [ ] Constructor con `?? throw new ArgumentNullException` para TODAS las dependencias
- [ ] Validators usan `ServiceResponseMessageType.X` constants (no strings literales)
- [ ] Handlers usan `ServiceResponseMessageType.X` en respuestas
- [ ] Logger inyectado y usado en try-catch de todos los handlers
- [ ] Services inyectados (no DbContext) en handlers
- [ ] Validacion retorna ServiceResponse (no throw exceptions)
- [ ] AutoMapper profile registrado en DependencyInjection
- [ ] Swagger documentation completa para todos los endpoints

---

## 13. Mensajes de Respuesta (ServiceResponseMessage)

### 13.1 Mensajes de Exito

| Operacion | Mensaje | ErrorCode | HTTP Status |
|-----------|---------|-----------|-------------|
| Crear reward | "Reward creado correctamente" | 0000 | 200 OK |
| Actualizar reward | "Reward actualizado correctamente" | 0002 | 200 OK |
| Eliminar reward | "Reward eliminado correctamente" | 0003 | 200 OK |
| Reordenar rewards | "Recompensas reordenadas correctamente" | 0002 | 200 OK |
| Listar rewards | "Rewards encontrados" | 0000 | 200 OK |
| Obtener reward | "Reward encontrado" | 0000 | 200 OK |

### 13.2 Mensajes de Error

| Codigo | Mensaje | Uso |
|--------|---------|-----|
| 1001 | "El {campo} es obligatorio" | Campos requeridos vacios |
| 1002 | "El {campo} no puede superar los {max} caracteres" | MaxLength excedido |
| 1007 | "El {campo} debe ser mayor a {min}" | Rango invalido |
| 1011 | "El importe minimo debe ser mayor a 0" | ImporteMinimo <= 0 |
| 2003 | "Campania no encontrada" | CampaniaId no existe |
| 2004 | "Reward no encontrado" | RewardId no existe |
| 3001 | "Token no valido o expirado" | JWT invalido |
| 3002 | "No tienes permiso para {accion} esta recompensa" | ArtistaId mismatch |
| 4010 | "No se puede {accion} una recompensa con aportes existentes" | Reward tiene backings |
| 5000 | "Error inesperado al {accion} reward" | Excepcion no controlada |

---

## 14. Notas de Implementacion

### 14.1 Auto-incrementar Orden

Al crear una nueva recompensa sin especificar `Orden`, el Handler debe calcular automaticamente:

```csharp
// Si Orden no se proporciona o es 0, auto-incrementar
if (request.Orden == 0)
{
    var maxOrden = await _rewardService.GetMaxOrderByCampaniaAsync(campaniaId, cancellationToken);
    entity.Orden = maxOrden + 1;
}
else
{
    entity.Orden = request.Orden;
}
```

### 14.2 Soft Delete

El `DeleteRewardCommandHandler` debe hacer soft delete:

```csharp
reward.EsActivo = false;
reward.FechaActualizacion = DateTime.UtcNow;
await _rewardService.UpdateAsync(reward, cancellationToken);
```

**NO** usar `DeleteAsync()` del service.

### 14.3 PATCH Semantics en Update

El `UpdateRewardCommandHandler` debe actualizar solo los campos proporcionados:

```csharp
// Obtener entidad actual
var reward = await _rewardService.GetByIdAsync(rewardId, cancellationToken);

// Actualizar solo campos no nulos
if (request.Nombre != null) reward.Nombre = request.Nombre;
if (request.Descripcion != null) reward.Descripcion = request.Descripcion;
if (request.ImporteMinimo.HasValue) reward.ImporteMinimo = request.ImporteMinimo.Value;
// ... etc

// O usar AutoMapper con .ForAllMembers(opts => opts.Condition(...))
_mapper.Map(request, reward);

// Actualizar
await _rewardService.UpdateAsync(reward, cancellationToken);
```

### 14.4 Reordenamiento en Lote

El `ReorderRewardsCommandHandler` debe actualizar todos los ordenes en una sola transaccion:

```csharp
// Validar todos los rewards primero
foreach (var order in request.RewardOrders)
{
    var reward = await _rewardService.GetByIdAsync(order.RewardId, cancellationToken);
    if (reward == null) return NotFound(...);
    if (reward.CampaniaId != request.CampaniaId) return Forbidden(...);
}

// Actualizar todos los ordenes
foreach (var order in request.RewardOrders)
{
    var reward = await _rewardService.GetByIdAsync(order.RewardId, cancellationToken);
    reward.Orden = order.Orden;
    await _rewardService.UpdateAsync(reward, cancellationToken);
}
```

**Alternativa:** Usar transaccion explicita en el service para rollback automatico en caso de error.

### 14.5 Orden de Campos en Validacion

Los validators deben validar en este orden:
1. Campos requeridos (NotEmpty)
2. Longitud (MaxLength/MinLength)
3. Formato (Email, Url, etc.)
4. Rango (GreaterThan, Between, etc.)
5. Logica de negocio (DependsOn, Custom rules)

---

## 15. Proximos Pasos Sugeridos

Una vez implementados los contratos API:

1. **Backend Agent** - Implementar handlers y validaciones
   - Crear `ReorderRewardsCommand.cs` con Handler
   - Crear `ReorderRewardsCommandValidator.cs`
   - Actualizar `RewardProfile.cs` con mapping UpdateCommand
   - Actualizar `RewardsController.cs` con endpoint reorder
   - Agregar validacion de ownership en handlers existentes
   - Agregar validacion de backings en Update/Delete handlers
   - Agregar codigo 4010 en `ServiceResponseMessageType.cs`

2. **Testing** - Unit tests para nuevos commands
   - `ReorderRewardsCommandHandlerTests`
   - `ReorderRewardsCommandValidatorTests`
   - Tests de validacion de ownership
   - Tests de validacion de backings

3. **Admin Dashboard** - UI de gestion de recompensas
   - Componente de drag & drop para reordenar
   - Formulario de creacion/edicion
   - Validacion de ownership en frontend

4. **Landing** - Vista publica de recompensas
   - Lista de recompensas en detalle de campania
   - Badge "Agotado" para stock 0

---

**Fin del plan de contratos API.**
