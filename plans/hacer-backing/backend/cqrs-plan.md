# Plan CQRS: Hacer Backing

**Fecha:** 2026-02-13
**Modulo:** Crowdfunding
**Feature:** hacer-backing

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Request | Response |
|-----------|------|---------|----------|
| Crear backing | Command | CreateBackingCommand | ServiceResponse&lt;BackingDto&gt; |
| Obtener detalle campania | Query | GetCampaniaDetailQuery | ServiceResponse&lt;CampaniaDetailDto&gt; |
| Listar backings de campania | Query | GetBackingsByCampaniaQuery | ServiceResponse&lt;PaginatedList&lt;BackingPublicDto&gt;&gt; |
| Obtener estadisticas | Query | GetCampaniaStatsQuery | ServiceResponse&lt;CampaniaStatsDto&gt; |

---

## 2. Commands

### 2.1 CreateBackingCommand

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Backings/Commands/CreateBackingCommand.cs`

**Contiene:** Command + Handler (MISMO archivo)

#### Command

```csharp
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;

namespace WePlayRises.Crowdfunding.Application.Features.Backings.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class CreateBackingCommand : IRequest<ServiceResponse<BackingDto>>
{
    /// <summary>
    /// ID de la campania a la que se hace el aporte (desde URL del endpoint).
    /// </summary>
    public Guid CampaniaId { get; set; }

    /// <summary>
    /// ID del reward seleccionado (opcional, null para aporte sin recompensa).
    /// </summary>
    public Guid? RewardId { get; set; }

    /// <summary>
    /// Monto del aporte en EUR (>= 1.00, >= reward.ImporteMinimo si reward seleccionado).
    /// </summary>
    public decimal Monto { get; set; }

    /// <summary>
    /// Mensaje opcional del backer para el artista (max 500 caracteres).
    /// </summary>
    public string? Mensaje { get; set; }

    /// <summary>
    /// Si true, el nombre del backer NO se muestra en la lista publica.
    /// </summary>
    public bool EsAnonimo { get; set; }

    // ===== PROPIEDADES INYECTADAS DESDE CONTROLLER =====

    /// <summary>
    /// ID del usuario autenticado (claim "sub" del JWT).
    /// Null si el usuario no esta autenticado.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Nombre del usuario autenticado (claim "name" del JWT).
    /// Null si el usuario no esta autenticado.
    /// </summary>
    public string? UserName { get; set; }
}
```

**Implementa:** `IRequest<ServiceResponse<BackingDto>>`

#### Handler

**Dependencias:**
```csharp
private readonly IBackingService _backingService;
private readonly ICampaniaService _campaniaService;
private readonly IRewardService _rewardService;
private readonly IMapper _mapper;
private readonly IValidator<CreateBackingCommand> _validator;
private readonly ILogger<CreateBackingCommandHandler> _logger;
```

**Constructor:**
```csharp
public CreateBackingCommandHandler(
    IBackingService backingService,
    ICampaniaService campaniaService,
    IRewardService rewardService,
    IMapper mapper,
    IValidator<CreateBackingCommand> validator,
    ILogger<CreateBackingCommandHandler> logger)
{
    _backingService = backingService ?? throw new ArgumentNullException(nameof(backingService));
    _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
    _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Flujo del Handler:**

1. **Validar** request con FluentValidation
2. Si invalido, retornar ServiceResponse con errores usando `GetServiceResponseMessages()`
3. **Cargar datos necesarios** (via services con cache):
   - Campania por ID
   - Reward por ID (si aplica)
4. **Llamar BackingService.CreateBackingAsync** (transaccion atomica):
   - Crear PedidoCrowdfunding
   - Crear PedidoCrowdfundingLinea
   - Actualizar CampaniaCrowdfunding.ImportePledgedActual
   - Crear AportacionCrowdfunding simulada
5. **Obtener pedido creado** via service
6. **Mapear a BackingDto** con AutoMapper
7. **Resolver campos custom** (no mapeables):
   - `CampaniaTitulo` → `campania.Titulo`
   - `RewardNombre` → `reward?.Nombre`
   - `MonedaSimbolo` → Lookup MaestraMoneda (hardcoded "EUR" en MVP)
   - `EstadoPedido` → Lookup MaestraEstadoPedido (hardcoded "Completado" en MVP)
   - `UserName` → `request.EsAnonimo || string.IsNullOrEmpty(request.UserId) ? "Anonimo" : request.UserName`
8. **Retornar ServiceResponse exitoso** con `ServiceResponseMessageType.Created`
9. **Try-catch** con logging para errores

**Pseudocodigo completo:**

```csharp
public async Task<ServiceResponse<BackingDto>> Handle(
    CreateBackingCommand request,
    CancellationToken cancellationToken)
{
    try
    {
        // 1. Validar
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CreateBacking: {Errors}",
                string.Join(", ", validationResult.Errors));

            return new ServiceResponse<BackingDto>
            {
                Messages = validationResult.GetServiceResponseMessages()
            };
        }

        // 2. Cargar datos necesarios (cache via services)
        var campaniaId = new CampaniaCrowdfundingId(request.CampaniaId);
        var campania = await _campaniaService.GetByIdAsync(campaniaId, cancellationToken);

        CampaniaCrowdfundingReward? reward = null;
        if (request.RewardId.HasValue)
        {
            var rewardId = new CampaniaCrowdfundingRewardId(request.RewardId.Value);
            reward = await _rewardService.GetByIdAsync(rewardId, cancellationToken);
        }

        // 3. Crear backing via servicio (transaccion atomica)
        var pedidoId = await _backingService.CreateBackingAsync(
            campaniaId,
            request.RewardId.HasValue ? new CampaniaCrowdfundingRewardId(request.RewardId.Value) : null,
            request.Monto,
            request.UserId,
            request.Mensaje,
            request.EsAnonimo,
            cancellationToken);

        // 4. Obtener pedido creado
        var pedidoService = _serviceProvider.GetRequiredService<IPedidoService>();
        var pedido = await pedidoService.GetByIdAsync(pedidoId, cancellationToken);

        // 5. Mapear a DTO
        var dto = _mapper.Map<BackingDto>(pedido);

        // 6. Resolver campos custom
        dto.CampaniaTitulo = campania!.Titulo;
        dto.RewardNombre = reward?.Nombre;
        dto.MonedaSimbolo = "EUR"; // MVP: Solo EUR
        dto.EstadoPedido = "Completado"; // MVP: Siempre completado
        dto.UserName = request.EsAnonimo || string.IsNullOrEmpty(request.UserId)
            ? "Anonimo"
            : request.UserName;

        _logger.LogInformation("Backing created: Pedido {PedidoId}, Campania {CampaniaId}, Monto {Monto}",
            pedidoId.Value, request.CampaniaId, request.Monto);

        // 7. Retornar exito
        return new ServiceResponse<BackingDto>
        {
            Data = dto,
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Aporte realizado con exito. Gracias por tu apoyo!",
                    ErrorCode = ServiceResponseMessageType.Created,
                    HttpStatusCode = System.Net.HttpStatusCode.Created
                }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating Backing for Campania {CampaniaId}", request.CampaniaId);
        return ValidateExtensions.InternalServerErrorServiceResponse<BackingDto>(
            "Error inesperado al procesar el aporte",
            ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
```

**NOTA IMPORTANTE:** El Handler NO inyecta DbContext. Usa `IBackingService` que encapsula la transaccion atomica. Ver `hexagonal-architecture.md` para implementacion de `BackingService.CreateBackingAsync`.

---

## 3. Queries

### 3.1 GetCampaniaDetailQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaDetailQuery.cs`

**Contiene:** Query + Handler (MISMO archivo)

#### Query

```csharp
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetCampaniaDetailQuery : IRequest<ServiceResponse<CampaniaDetailDto>>
{
    public Guid Id { get; set; }
}
```

**Implementa:** `IRequest<ServiceResponse<CampaniaDetailDto>>`

#### Handler

**Dependencias:**
```csharp
private readonly ICampaniaService _campaniaService;
private readonly IRewardService _rewardService;
private readonly IPedidoService _pedidoService;
private readonly IMapper _mapper;
private readonly ILogger<GetCampaniaDetailQueryHandler> _logger;
```

**Constructor:**
```csharp
public GetCampaniaDetailQueryHandler(
    ICampaniaService campaniaService,
    IRewardService rewardService,
    IPedidoService pedidoService,
    IMapper mapper,
    ILogger<GetCampaniaDetailQueryHandler> logger)
{
    _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
    _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
    _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Flujo:**

1. Llamar `CampaniaService.GetDetailByIdAsync(id, ct)` (retorna entidad con includes: Rewards activos, Pedidos recientes)
2. Si null, retornar NOT_FOUND con `ValidateExtensions.NotFoundServiceResponse`
3. Mapear `CampaniaCrowdfunding → CampaniaDetailDto` con AutoMapper
4. **Calcular campos custom:**
   - `PorcentajeProgreso = (ImportePledgedActual / ImporteObjetivo) * 100`
   - `DiasRestantes = Math.Max(0, (FechaFin - DateTime.UtcNow).Days)`
   - `MonedaSimbolo = "EUR"` (hardcoded MVP)
   - `EstadoCampaniaNombre = "Publicada"` (lookup via MaestraEstadoCampania en futuro)
   - `ArtistaNombre`, `ArtistaImagenUrl` → Resolver via Artista navigation property o service
5. **Resolver colecciones:**
   - `Rewards` → Mapear lista de `CampaniaCrowdfundingReward → RewardPublicDto`
   - Para cada reward, calcular `CantidadVendida` via `RewardService.GetCantidadVendidaAsync`
   - Para cada reward, calcular `Disponible = CantidadMaxima == null || CantidadVendida < CantidadMaxima`
   - `BackingsRecientes` → Mapear lista de `PedidoCrowdfunding → BackingPublicDto`
   - Para cada backing, resolver `NombreBacker` via UserService o "Anonimo"
   - `TotalBackers` → `CampaniaService.GetTotalBackersAsync(campaniaId, ct)`
6. Retornar ServiceResponse con DTO

**Pseudocodigo:**

```csharp
public async Task<ServiceResponse<CampaniaDetailDto>> Handle(
    GetCampaniaDetailQuery request,
    CancellationToken cancellationToken)
{
    try
    {
        var id = new CampaniaCrowdfundingId(request.Id);
        var campania = await _campaniaService.GetDetailByIdAsync(id, cancellationToken);

        if (campania == null)
        {
            return ValidateExtensions.NotFoundServiceResponse<CampaniaDetailDto>(
                "Campania no encontrada",
                ServiceResponseMessageType.NotFound_Campania);
        }

        // Mapear datos base
        var dto = _mapper.Map<CampaniaDetailDto>(campania);

        // Calcular metricas
        dto.PorcentajeProgreso = campania.ImporteObjetivo > 0
            ? Math.Round((campania.ImportePledgedActual / campania.ImporteObjetivo) * 100, 1)
            : 0;
        dto.DiasRestantes = campania.FechaFin.HasValue
            ? Math.Max(0, (campania.FechaFin.Value - DateTime.UtcNow).Days)
            : 0;
        dto.MonedaSimbolo = "EUR"; // MVP: Solo EUR
        dto.EstadoCampaniaNombre = "Publicada"; // TODO: Lookup MaestraEstadoCampania

        // Resolver artista (via navigation property o service)
        dto.ArtistaNombre = campania.Artista?.NombreArtistico ?? "Artista Desconocido";
        dto.ArtistaImagenUrl = campania.Artista?.ImagenPerfilUrl;

        // Mapear rewards con stock
        dto.Rewards = new List<RewardPublicDto>();
        foreach (var reward in campania.Rewards.Where(r => r.EsActivo).OrderBy(r => r.Orden))
        {
            var rewardDto = _mapper.Map<RewardPublicDto>(reward);
            rewardDto.CantidadVendida = await _rewardService.GetCantidadVendidaAsync(reward.Id, cancellationToken);
            rewardDto.Disponible = reward.CantidadMaxima == null || rewardDto.CantidadVendida < reward.CantidadMaxima.Value;
            dto.Rewards.Add(rewardDto);
        }

        // Mapear backings recientes (ultimos 10)
        dto.BackingsRecientes = new List<BackingPublicDto>();
        foreach (var pedido in campania.Pedidos.OrderByDescending(p => p.FechaCreacion).Take(10))
        {
            var backingDto = _mapper.Map<BackingPublicDto>(pedido);
            backingDto.NombreBacker = pedido.PermitirMostrarNombre && !string.IsNullOrEmpty(pedido.UserId)
                ? await GetUserNameAsync(pedido.UserId, cancellationToken) // TODO: UserService
                : "Anonimo";
            backingDto.RewardNombre = pedido.Lineas.FirstOrDefault()?.Reward?.Nombre;
            dto.BackingsRecientes.Add(backingDto);
        }

        // Total backers
        dto.TotalBackers = await _campaniaService.GetTotalBackersAsync(id, cancellationToken);

        return new ServiceResponse<CampaniaDetailDto>
        {
            Data = dto,
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Campania encontrada",
                    HttpStatusCode = System.Net.HttpStatusCode.OK
                }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting Campania detail for Id {Id}", request.Id);
        return ValidateExtensions.InternalServerErrorServiceResponse<CampaniaDetailDto>(
            "Error inesperado",
            ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
```

---

### 3.2 GetBackingsByCampaniaQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Backings/Queries/GetBackingsByCampaniaQuery.cs`

**Contiene:** Query + Handler (MISMO archivo)

#### Query

```csharp
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Dtos.Common;

namespace WePlayRises.Crowdfunding.Application.Features.Backings.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetBackingsByCampaniaQuery : IRequest<ServiceResponse<PaginatedList<BackingPublicDto>>>
{
    public Guid CampaniaId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
```

**Implementa:** `IRequest<ServiceResponse<PaginatedList<BackingPublicDto>>>`

#### Handler

**Dependencias:**
```csharp
private readonly IPedidoService _pedidoService;
private readonly IMapper _mapper;
private readonly IValidator<GetBackingsByCampaniaQuery> _validator;
private readonly ILogger<GetBackingsByCampaniaQueryHandler> _logger;
```

**Flujo:**

1. Validar query con FluentValidation (PageNumber > 0, PageSize entre 1-100)
2. Llamar `PedidoService.GetByCampaniaIdPaginatedAsync(campaniaId, pageNumber, pageSize, ct)`
3. Filtrar solo pedidos con `EstadoPedidoId = 3` (COMPLETADO)
4. Ordenar por `FechaCreacion DESC`
5. Mapear cada `PedidoCrowdfunding → BackingPublicDto`
6. Resolver `NombreBacker` y `RewardNombre` para cada item
7. Retornar `PaginatedList<BackingPublicDto>` con metadatos

---

### 3.3 GetCampaniaStatsQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaStatsQuery.cs`

**Contiene:** Query + Handler (MISMO archivo)

#### Query

```csharp
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetCampaniaStatsQuery : IRequest<ServiceResponse<CampaniaStatsDto>>
{
    public Guid CampaniaId { get; set; }
}
```

**Implementa:** `IRequest<ServiceResponse<CampaniaStatsDto>>`

#### Handler

**Dependencias:**
```csharp
private readonly ICampaniaService _campaniaService;
private readonly IPedidoService _pedidoService;
private readonly ILogger<GetCampaniaStatsQueryHandler> _logger;
```

**Flujo:**

1. Verificar que campania existe
2. Obtener todos los pedidos completados de la campania
3. Calcular estadisticas:
   - `TotalBackers = pedidos.Count()`
   - `TotalRecaudado = pedidos.Sum(p => p.ImporteTotal)`
   - `PromedioAporte = pedidos.Average(p => p.ImporteTotal)` (o 0 si no hay)
   - `AporteMinimo = pedidos.Min(p => p.ImporteTotal)` (o 0 si no hay)
   - `AporteMaximo = pedidos.Max(p => p.ImporteTotal)` (o 0 si no hay)
   - `DiasRestantes = Math.Max(0, (campania.FechaFin - DateTime.UtcNow).Days)`
4. Retornar `CampaniaStatsDto`

---

## 4. Validators

### 4.1 CreateBackingCommandValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Backings/Validators/CreateBackingCommandValidator.cs`

**Reglas de Validacion:**

| Campo | Regla | Mensaje | ErrorCode (Constant) | Tipo |
|-------|-------|---------|----------------------|------|
| CampaniaId | NotEmpty | El ID de campania es obligatorio | `ServiceResponseMessageType.Validation_Required` | Sincrona |
| Monto | GreaterThanOrEqualTo(1) | El monto debe ser al menos 1 EUR | `ServiceResponseMessageType.Validation_InvalidAmount` | Sincrona |
| Mensaje | MaximumLength(500) | El mensaje no puede superar los 500 caracteres | `ServiceResponseMessageType.Validation_MaxLength` | Sincrona |
| CampaniaId | MustAsync: campania existe | Campania no encontrada | `ServiceResponseMessageType.NotFound_Campania` | Asincrona |
| CampaniaId | MustAsync: estado PUBLICADA | Esta campania no esta activa | `ServiceResponseMessageType.BusinessRule_CampaniaNotActive` | Asincrona |
| CampaniaId | MustAsync: FechaFin no expirada | Esta campania ya finalizo | `ServiceResponseMessageType.BusinessRule_CampaniaEnded` | Asincrona |
| UserId | MustAsync: validar auth si no permite anonimos | Debes iniciar sesion para hacer un aporte | `ServiceResponseMessageType.Auth_UserNotAuthenticated` | Asincrona |
| RewardId | MustAsync: reward existe y activo | Recompensa no encontrada | `ServiceResponseMessageType.NotFound_Reward` | Asincrona |
| RewardId | MustAsync: reward tiene stock | Esta recompensa esta agotada | `ServiceResponseMessageType.BusinessRule_RewardOutOfStock` | Asincrona |
| Monto | MustAsync: monto >= reward.ImporteMinimo | El monto debe ser al menos {ImporteMinimo} EUR para esta recompensa | `ServiceResponseMessageType.BusinessRule_AmountBelowMinimum` | Asincrona |

**NUEVAS CONSTANTES A AGREGAR EN ServiceResponseMessageType.cs:**

```csharp
// Business Rule errors (4000-4999) - AGREGAR
public const string BusinessRule_RewardOutOfStock = "4011";
public const string BusinessRule_AmountBelowMinimum = "4012";
public const string BusinessRule_AnonymousNotAllowed = "4013";
```

**Dependencias:**
```csharp
private readonly ICampaniaService _campaniaService;
private readonly IRewardService _rewardService;
private readonly ILogger<CreateBackingCommandValidator> _logger;

public CreateBackingCommandValidator(
    ICampaniaService campaniaService,
    IRewardService rewardService,
    ILogger<CreateBackingCommandValidator> logger)
{
    _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
    _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    // Rules setup...
}
```

**Pseudocodigo de Reglas:**

```csharp
// 1. Validaciones sincronas
RuleFor(x => x.CampaniaId)
    .NotEmpty()
    .WithMessage("El ID de campania es obligatorio")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required);

RuleFor(x => x.Monto)
    .GreaterThanOrEqualTo(1)
    .WithMessage("El monto debe ser al menos 1 EUR")
    .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);

RuleFor(x => x.Mensaje)
    .MaximumLength(500)
    .WithMessage("El mensaje no puede superar los 500 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
    .When(x => !string.IsNullOrEmpty(x.Mensaje));

// 2. Validar campania existe y esta activa
RuleFor(x => x.CampaniaId)
    .MustAsync(async (campaniaId, ct) =>
    {
        var id = new CampaniaCrowdfundingId(campaniaId);
        var campania = await _campaniaService.GetByIdAsync(id, ct);
        return campania != null;
    })
    .WithMessage("Campania no encontrada")
    .WithErrorCode(ServiceResponseMessageType.NotFound_Campania);

RuleFor(x => x.CampaniaId)
    .MustAsync(async (campaniaId, ct) =>
    {
        var id = new CampaniaCrowdfundingId(campaniaId);
        var campania = await _campaniaService.GetByIdAsync(id, ct);
        return campania?.EstadoCampaniaId == 2; // PUBLICADA
    })
    .WithMessage("Esta campania no esta activa")
    .WithErrorCode(ServiceResponseMessageType.BusinessRule_CampaniaNotActive);

RuleFor(x => x.CampaniaId)
    .MustAsync(async (campaniaId, ct) =>
    {
        var id = new CampaniaCrowdfundingId(campaniaId);
        var campania = await _campaniaService.GetByIdAsync(id, ct);
        return !campania.FechaFin.HasValue || DateTime.UtcNow <= campania.FechaFin.Value;
    })
    .WithMessage("Esta campania ya finalizo")
    .WithErrorCode(ServiceResponseMessageType.BusinessRule_CampaniaEnded);

// 3. Validar autenticacion si campania no permite anonimos
RuleFor(x => x)
    .MustAsync(async (command, ct) =>
    {
        if (!string.IsNullOrEmpty(command.UserId)) return true; // Autenticado, OK

        var id = new CampaniaCrowdfundingId(command.CampaniaId);
        var campania = await _campaniaService.GetByIdAsync(id, ct);
        return campania?.PermiteAportacionesAnonimas == true;
    })
    .WithMessage("Debes iniciar sesion para hacer un aporte")
    .WithErrorCode(ServiceResponseMessageType.Auth_UserNotAuthenticated);

// 4. Validar reward (solo si RewardId != null)
When(x => x.RewardId.HasValue, () =>
{
    RuleFor(x => x.RewardId!.Value)
        .MustAsync(async (rewardId, ct) =>
        {
            var id = new CampaniaCrowdfundingRewardId(rewardId);
            var reward = await _rewardService.GetByIdAsync(id, ct);
            return reward != null && reward.EsActivo;
        })
        .WithMessage("Recompensa no encontrada")
        .WithErrorCode(ServiceResponseMessageType.NotFound_Reward);

    RuleFor(x => x.RewardId!.Value)
        .MustAsync(async (rewardId, ct) =>
        {
            var id = new CampaniaCrowdfundingRewardId(rewardId);
            return await _rewardService.HasStockAvailableAsync(id, ct);
        })
        .WithMessage("Esta recompensa esta agotada")
        .WithErrorCode(ServiceResponseMessageType.BusinessRule_RewardOutOfStock);

    RuleFor(x => x.Monto)
        .MustAsync(async (command, monto, ct) =>
        {
            var id = new CampaniaCrowdfundingRewardId(command.RewardId!.Value);
            var reward = await _rewardService.GetByIdAsync(id, ct);
            return reward == null || monto >= reward.ImporteMinimo;
        })
        .WithMessage((command, monto) =>
        {
            var id = new CampaniaCrowdfundingRewardId(command.RewardId!.Value);
            var reward = _rewardService.GetByIdAsync(id, CancellationToken.None).Result;
            return $"El monto debe ser al menos {reward?.ImporteMinimo} EUR para esta recompensa";
        })
        .WithErrorCode(ServiceResponseMessageType.BusinessRule_AmountBelowMinimum);
});
```

**NOTA CRITICA:** Las validaciones asincronas usan `IRequestCacheService` via Services para evitar queries duplicados (ADR-006). El Validator llama `_campaniaService.GetByIdAsync` → Cache MISS → DB query. Luego el Handler llama `_campaniaService.GetByIdAsync` → Cache HIT → Sin DB query.

---

### 4.2 GetBackingsByCampaniaQueryValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Backings/Validators/GetBackingsByCampaniaQueryValidator.cs`

**Reglas:**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| CampaniaId | NotEmpty | El ID de campania es obligatorio | `ServiceResponseMessageType.Validation_Required` |
| PageNumber | GreaterThan(0) | El numero de pagina debe ser mayor a 0 | `ServiceResponseMessageType.Validation_InvalidRange` |
| PageSize | InclusiveBetween(1, 100) | El tamano de pagina debe estar entre 1 y 100 | `ServiceResponseMessageType.Validation_InvalidRange` |

**Dependencias:** Ninguna (solo validaciones sincronas).

```csharp
public class GetBackingsByCampaniaQueryValidator : AbstractValidator<GetBackingsByCampaniaQuery>
{
    public GetBackingsByCampaniaQueryValidator()
    {
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("El ID de campania es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("El numero de pagina debe ser mayor a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("El tamano de pagina debe estar entre 1 y 100")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }
}
```

---

### 4.3 GetCampaniaStatsQueryValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/GetCampaniaStatsQueryValidator.cs`

**Reglas:**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| CampaniaId | NotEmpty | El ID de campania es obligatorio | `ServiceResponseMessageType.Validation_Required` |

**Dependencias:** Ninguna.

```csharp
public class GetCampaniaStatsQueryValidator : AbstractValidator<GetCampaniaStatsQuery>
{
    public GetCampaniaStatsQueryValidator()
    {
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("El ID de campania es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
```

---

## 5. AutoMapper Mappings

### 5.1 BackingProfile (NUEVO)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/BackingProfile.cs`

```csharp
using AutoMapper;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Backings.Commands;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Mapping;

public class BackingProfile : Profile
{
    public BackingProfile()
    {
        // PedidoCrowdfunding -> BackingDto
        CreateMap<PedidoCrowdfunding, BackingDto>()
            .ForMember(dest => dest.Monto, opt => opt.MapFrom(src => src.ImporteTotal))
            .ForMember(dest => dest.Mensaje, opt => opt.MapFrom(src => src.ComentarioBacker))
            .ForMember(dest => dest.EsAnonimo, opt => opt.MapFrom(src => !src.PermitirMostrarNombre))
            .ForMember(dest => dest.RewardId, opt => opt.MapFrom(src =>
                src.Lineas.FirstOrDefault(l => l.EsRewardPrincipal) != null
                    ? src.Lineas.First(l => l.EsRewardPrincipal).RewardId.Value
                    : (Guid?)null))
            .ForMember(dest => dest.CampaniaTitulo, opt => opt.Ignore()) // Resolver en Handler
            .ForMember(dest => dest.RewardNombre, opt => opt.Ignore()) // Resolver en Handler
            .ForMember(dest => dest.MonedaSimbolo, opt => opt.Ignore()) // Resolver en Handler
            .ForMember(dest => dest.EstadoPedido, opt => opt.Ignore()) // Resolver en Handler
            .ForMember(dest => dest.UserName, opt => opt.Ignore()); // Resolver en Handler

        // PedidoCrowdfunding -> BackingPublicDto
        CreateMap<PedidoCrowdfunding, BackingPublicDto>()
            .ForMember(dest => dest.Monto, opt => opt.MapFrom(src => src.ImporteTotal))
            .ForMember(dest => dest.Mensaje, opt => opt.MapFrom(src => src.ComentarioBacker))
            .ForMember(dest => dest.NombreBacker, opt => opt.Ignore()) // Resolver en Handler
            .ForMember(dest => dest.RewardNombre, opt => opt.Ignore()); // Resolver en Handler
    }
}
```

**NOTA:** Campos marcados con `opt.Ignore()` requieren logica custom en el Handler porque:
- `RewardNombre`: Requiere join o query adicional a `CampaniaCrowdfundingReward`.
- `MonedaSimbolo`, `EstadoPedido`: Requieren lookups a tablas maestras (hardcoded en MVP).
- `UserName`, `NombreBacker`: Requieren llamada a `UserService` o devolver `"Anonimo"`.

---

### 5.2 CampaniaProfile (EXTENDER)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/CampaniaProfile.cs`

**Mappings a AGREGAR:**

```csharp
// CampaniaCrowdfunding -> CampaniaDetailDto
CreateMap<CampaniaCrowdfunding, CampaniaDetailDto>()
    .ForMember(dest => dest.PorcentajeProgreso, opt => opt.Ignore()) // Calcular en Handler
    .ForMember(dest => dest.DiasRestantes, opt => opt.Ignore()) // Calcular en Handler
    .ForMember(dest => dest.MonedaSimbolo, opt => opt.Ignore()) // Resolver en Handler
    .ForMember(dest => dest.EstadoCampaniaNombre, opt => opt.Ignore()) // Resolver en Handler
    .ForMember(dest => dest.ArtistaNombre, opt => opt.Ignore()) // Resolver en Handler
    .ForMember(dest => dest.ArtistaImagenUrl, opt => opt.Ignore()) // Resolver en Handler
    .ForMember(dest => dest.Rewards, opt => opt.Ignore()) // Resolver en Handler
    .ForMember(dest => dest.BackingsRecientes, opt => opt.Ignore()) // Resolver en Handler
    .ForMember(dest => dest.TotalBackers, opt => opt.Ignore()); // Resolver en Handler
```

**NOTA:** Casi todos los campos custom se resuelven en el Handler porque requieren:
- Calculos dinamicos (PorcentajeProgreso, DiasRestantes)
- Queries adicionales (TotalBackers, CantidadVendida)
- Lookups de maestras (MonedaSimbolo, EstadoCampaniaNombre)
- Navegacion a otras entidades (ArtistaNombre)

---

### 5.3 RewardProfile (EXTENDER)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/RewardProfile.cs`

**Mappings a AGREGAR:**

```csharp
// CampaniaCrowdfundingReward -> RewardPublicDto
CreateMap<CampaniaCrowdfundingReward, RewardPublicDto>()
    .ForMember(dest => dest.CantidadVendida, opt => opt.Ignore()) // Calcular en Handler via RewardService
    .ForMember(dest => dest.Disponible, opt => opt.Ignore()); // Calcular en Handler (CantidadMaxima == null || CantidadVendida < CantidadMaxima)
```

---

## 6. Archivos a Crear

```
Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
├── Features/
│   ├── Backings/                                       # NUEVO directorio
│   │   ├── Commands/
│   │   │   └── CreateBackingCommand.cs                # NUEVO - Command + Handler
│   │   ├── Queries/
│   │   │   └── GetBackingsByCampaniaQuery.cs          # NUEVO - Query + Handler
│   │   └── Validators/
│   │       ├── CreateBackingCommandValidator.cs       # NUEVO
│   │       └── GetBackingsByCampaniaQueryValidator.cs # NUEVO
│   └── Campanias/
│       ├── Queries/
│       │   ├── GetCampaniaDetailQuery.cs              # NUEVO - Query + Handler
│       │   └── GetCampaniaStatsQuery.cs               # NUEVO - Query + Handler
│       └── Validators/
│           ├── GetCampaniaDetailQueryValidator.cs     # NUEVO (opcional, solo valida ID)
│           └── GetCampaniaStatsQueryValidator.cs      # NUEVO
├── Dtos/
│   ├── BackingDto.cs                                   # NUEVO
│   ├── BackingPublicDto.cs                             # NUEVO
│   ├── CampaniaDetailDto.cs                            # NUEVO
│   ├── CampaniaStatsDto.cs                             # NUEVO
│   └── RewardPublicDto.cs                              # NUEVO
└── Mapping/
    ├── BackingProfile.cs                               # NUEVO
    ├── CampaniaProfile.cs                              # MODIFICAR (agregar mappings)
    └── RewardProfile.cs                                # MODIFICAR (agregar mappings)
```

**Archivos a MODIFICAR:**
- `ServiceResponseMessageType.cs` - Agregar 3 constantes nuevas (4011, 4012, 4013)
- `CampaniaProfile.cs` - Agregar mapping `CampaniaCrowdfunding -> CampaniaDetailDto`
- `RewardProfile.cs` - Agregar mapping `CampaniaCrowdfundingReward -> RewardPublicDto`

---

## 7. Patrones Importantes

### 7.1 Logica de Negocio en Handler

```
Handler contiene:
- Validacion (via FluentValidation)
- Logica de negocio (calculos, reglas, transformaciones)
- Orquestacion de llamadas a services
- Mapeo de entidades a DTOs

Service contiene:
- SOLO persistencia (CRUD via repository)
- Transacciones atomicas (para operaciones multi-entidad)
- NO logica de negocio
```

**Excepcion:** `BackingService.CreateBackingAsync` encapsula transaccion atomica porque involucra multiples entidades:
1. `PedidoCrowdfunding`
2. `PedidoCrowdfundingLinea`
3. `CampaniaCrowdfunding.ImportePledgedActual` (update)
4. `AportacionCrowdfunding`

Estas 4 operaciones deben ser atomicas (todo o nada).

---

### 7.2 ServiceResponse con Constants

```csharp
// Exito - USAR CONSTANTS
return new ServiceResponse<BackingDto>
{
    Data = dto,
    Messages = new List<ServiceResponseMessage>
    {
        new()
        {
            Message = "Aporte realizado con exito",
            ErrorCode = ServiceResponseMessageType.Created,
            HttpStatusCode = System.Net.HttpStatusCode.Created
        }
    }
};

// Error de validacion
return new ServiceResponse<BackingDto>
{
    Messages = validationResult.GetServiceResponseMessages()
};

// Error de negocio - USAR CONSTANTS
return ValidateExtensions.NotFoundServiceResponse<BackingDto>(
    "Campania no encontrada",
    ServiceResponseMessageType.NotFound_Campania);
```

---

### 7.3 Request Caching para Evitar Queries Duplicados

Flujo con cache (ADR-006):

```
1. CreateBackingCommandValidator.Validate(request):
   → _campaniaService.GetByIdAsync(id)
     → RequestCache MISS → Repository → DB
     → Cache almacena resultado

2. CreateBackingCommandHandler.Handle(request):
   → _campaniaService.GetByIdAsync(id)
     → RequestCache HIT → Retorna inmediato (sin DB query)
```

**Beneficio:** Validator y Handler comparten datos sin queries duplicados a DB.

---

### 7.4 Transaccion Atomica en BackingService

El `BackingService.CreateBackingAsync` usa transaccion explicita:

```csharp
await using var transaction = await _context.Database.BeginTransactionAsync(ct);

try
{
    // 1. Crear PedidoCrowdfunding
    // 2. Crear PedidoCrowdfundingLinea
    // 3. Actualizar CampaniaCrowdfunding.ImportePledgedActual
    // 4. Crear AportacionCrowdfunding
    await _context.SaveChangesAsync(ct);
    await transaction.CommitAsync(ct);
}
catch (Exception ex)
{
    await transaction.RollbackAsync(ct);
    throw;
}
```

**NOTA:** `BackingService` es la UNICA excepcion que inyecta `DbContext` para transacciones. Los demas Services usan Repositories con `SaveChanges` interno.

---

## 8. Checklist de Implementacion

- [ ] **Constantes:** Agregar 3 nuevas constantes (4011, 4012, 4013) a `ServiceResponseMessageType.cs`
- [ ] **DTOs:** Crear 5 nuevos DTOs (BackingDto, BackingPublicDto, CampaniaDetailDto, RewardPublicDto, CampaniaStatsDto)
- [ ] **Commands:** Crear `CreateBackingCommand.cs` con Command + Handler en MISMO archivo
- [ ] **Queries:** Crear 3 queries (GetCampaniaDetailQuery, GetBackingsByCampaniaQuery, GetCampaniaStatsQuery) con Query + Handler
- [ ] **Validators:** Crear 2 validators principales (CreateBackingCommandValidator, GetBackingsByCampaniaQueryValidator)
- [ ] **Mappings:** Crear `BackingProfile.cs` y extender `CampaniaProfile.cs` y `RewardProfile.cs`
- [ ] **Handler Logic:** Implementar logica completa en cada Handler con try-catch + logging
- [ ] **Validacion Asincrona:** Validators usan Services con RequestCache para evitar queries duplicados
- [ ] **Constructor:** TODAS las dependencias con `?? throw new ArgumentNullException`
- [ ] **ErrorCodes:** USAR `ServiceResponseMessageType.X` (NO strings literales)
- [ ] **Logging:** `_logger.LogError` en catch, `_logger.LogInformation` en exito
- [ ] **ValidateExtensions:** Usar helpers `NotFoundServiceResponse`, `InternalServerErrorServiceResponse`

---

## 9. Reglas CQRS Aplicadas

- ✓ **Regla 1:** Handler + Command en MISMO archivo (`CreateBackingCommand.cs`)
- ✓ **Regla 2:** SIEMPRE retornar `ServiceResponse<T>` (todos los handlers)
- ✓ **Regla 3:** Handler NUNCA inyecta DbContext - Handler usa `IBackingService`, Service encapsula transaccion
- ✓ **Regla 4:** Services retornan Entidades, NO DTOs
- ✓ **Regla 5:** Validators con `.WithMessage()` Y `.WithErrorCode(ServiceResponseMessageType.X)`
- ✓ **Regla 6:** Validators en carpeta `Validators/` separada
- ✓ **Regla 7:** Logica de negocio en Handlers, NO en Services (excepto transaccion atomica)
- ✓ **Regla 8:** Constructor con `?? throw new ArgumentNullException` para TODAS las dependencias
- ✓ **Regla 9:** USAR `ServiceResponseMessageType` constants (NO strings literales)
- ✓ **Regla 10:** Try-Catch con logging en handlers
- ✓ **Regla 11:** Validacion retorna ServiceResponse (NO throw)
- ✓ **Regla 12:** Request Caching via Services (ADR-006)

---

## 10. Proximos Pasos

Despues de disenar la arquitectura CQRS:

1. **Implementar Domain/Infrastructure** (ver `hexagonal-architecture.md`):
   - Crear `IBackingService` + `BackingService`
   - Extender repositorios con metodos nuevos
   - Agregar constantes en `ServiceResponseMessageType.cs`

2. **Implementar Application (CQRS):**
   - Crear 5 DTOs nuevos
   - Crear Commands/Queries con Handlers
   - Crear Validators con reglas asincronas
   - Crear/Extender AutoMapper Profiles

3. **Implementar Controllers:**
   - Crear `BackingsController` o extender `CampaniasController`
   - Endpoint POST `/api/campanias/{id}/backings`
   - Endpoint GET `/api/campanias/{id}/backings`
   - Endpoint GET `/api/campanias/{id}/stats`
   - Modificar GET `/api/campanias/{id}` para usar `GetCampaniaDetailQuery`

4. **Testing:**
   - Unit tests para Validators (todas las reglas)
   - Unit tests para Handlers (mocking services)
   - Integration tests para transaccion atomica de backing
   - E2E tests para flujo completo desde landing

---

**Fin del plan CQRS para hacer-backing.**
