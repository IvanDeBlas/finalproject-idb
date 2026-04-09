# Plan CQRS: cp-wallet-comisiones

**Fecha:** 2026-03-02
**Modulo:** Crowdpromotion
**Feature:** cp-wallet-comisiones (US-CP-06: Wallet de Promotor, Comisiones y Cobros)
**Depende de:** hexagonal-architecture.md, api-contracts.md

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Archivo | Request | Response |
|-----------|------|---------|---------|----------|
| Obtener wallet del promotor | Query | `GetPromotorWalletQuery.cs` | `GetPromotorWalletQuery` | `ServiceResponse<PromotorWalletDto>` |
| Listar transacciones paginadas | Query | `GetWalletTransaccionesQuery.cs` | `GetWalletTransaccionesQuery` | `ServiceResponse<WalletTransaccionesPagedDto>` |
| Solicitar cobro/retiro | Command | `SolicitarCobroCommand.cs` | `SolicitarCobroCommand` | `ServiceResponse<SolicitarCobroResponseDto>` |

---

## 2. Archivos a Crear

```
src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/
├── Features/
│   └── Wallet/
│       ├── Queries/
│       │   ├── GetPromotorWalletQuery.cs          (Query + Handler en el MISMO archivo)
│       │   └── GetWalletTransaccionesQuery.cs      (Query + Handler en el MISMO archivo)
│       ├── Commands/
│       │   └── SolicitarCobroCommand.cs            (Command + Handler en el MISMO archivo)
│       └── Validators/
│           ├── GetPromotorWalletQueryValidator.cs
│           ├── GetWalletTransaccionesQueryValidator.cs
│           └── SolicitarCobroCommandValidator.cs
├── Dtos/
│   ├── PromotorWalletDto.cs                       (ya planificado en api-contracts.md)
│   ├── WalletTransaccionItemDto.cs                (ya planificado en api-contracts.md)
│   ├── WalletTransaccionesPagedDto.cs             (ya planificado en api-contracts.md)
│   ├── SolicitarCobroRequestDto.cs                (ya planificado en api-contracts.md)
│   └── SolicitarCobroResponseDto.cs               (ya planificado en api-contracts.md)
└── Mapping/
    └── PromotorWalletProfile.cs                   (AutoMapper Profile nuevo)
```

---

## 3. Queries

### 3.1 GetPromotorWalletQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Queries/GetPromotorWalletQuery.cs`

**Contiene:** Query + Handler en el mismo archivo (REGLA 2)

**Tipo:** Query (solo lectura, sin side effects)

#### 3.1.1 Query

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `UserId` | `string` | Si | JWT claim `sub` | UserId del promotor autenticado. El Handler lo resuelve a PromotorId via IPromotorWalletService |

**Implementa:** `IRequest<ServiceResponse<PromotorWalletDto>>`

#### 3.1.2 Handler

**Clase:** `GetPromotorWalletQueryHandler`

**Dependencias inyectadas:**

| Campo | Tipo | Proposito |
|-------|------|-----------|
| `_walletService` | `IPromotorWalletService` | Resolver promotor y obtener wallet |
| `_mapper` | `IMapper` | Mapear `PromotorWallet` -> `PromotorWalletDto` |
| `_validator` | `IValidator<GetPromotorWalletQuery>` | Validar UserId presente |
| `_configuration` | `IConfiguration` | Leer `Crowdpromotion:MinimoRetiro` (default 10.0m) |
| `_logger` | `ILogger<GetPromotorWalletQueryHandler>` | Logging de errores |

**CRITICO:** Todas las dependencias con `?? throw new ArgumentNullException(nameof(...))` en el constructor.

**Flujo del Handle:**

```
1. Validar request con IValidator<GetPromotorWalletQuery>
   └─> !IsValid => return ServiceResponse con validationResult.GetServiceResponseMessages()

2. Resolver promotor via _walletService.GetPromotorByUserIdAsync(request.UserId, ct)
   └─> null => return NotFoundServiceResponse con NotFound_Promotor ("2015")
              mensaje: "No tienes un perfil de promotor"

3. Obtener wallet con moneda via _walletService.GetWalletConMonedaAsync(promotor.PromotorId, ct)
   (retorna tupla (PromotorWallet? Wallet, string? MonedaNombre))
   └─> wallet == null => return NotFoundServiceResponse con NotFound_Wallet ("2030")
                         mensaje: "No tienes un wallet asignado"

4. Mapear PromotorWallet -> PromotorWalletDto via _mapper.Map<PromotorWalletDto>(wallet)

5. Asignar manualmente dto.MinimoRetiro = _configuration.GetValue<decimal>("Crowdpromotion:MinimoRetiro", 10.0m)
   NOTA: MinimoRetiro NO viene de la entidad (no existe en PromotorWallet).
         AutoMapper lo ignora; el Handler lo asigna desde IConfiguration.

6. Retornar ServiceResponse<PromotorWalletDto> { Data = dto }
   (sin mensaje de exito explicito - solo Data)

7. try-catch: capturar Exception, LogError, retornar InternalServerErrorServiceResponse con Internal_UnexpectedError ("5000")
```

**Codigo del archivo completo:**

```csharp
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;

public class GetPromotorWalletQuery : IRequest<ServiceResponse<PromotorWalletDto>>
{
    /// <summary>
    /// UserId extraido del claim 'sub' del JWT.
    /// El Handler resuelve internamente al PromotorId via IPromotorWalletService.
    /// </summary>
    public string UserId { get; set; } = null!;
}

public class GetPromotorWalletQueryHandler
    : IRequestHandler<GetPromotorWalletQuery, ServiceResponse<PromotorWalletDto>>
{
    private readonly IPromotorWalletService _walletService;
    private readonly IMapper _mapper;
    private readonly IValidator<GetPromotorWalletQuery> _validator;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetPromotorWalletQueryHandler> _logger;

    public GetPromotorWalletQueryHandler(
        IPromotorWalletService walletService,
        IMapper mapper,
        IValidator<GetPromotorWalletQuery> validator,
        IConfiguration configuration,
        ILogger<GetPromotorWalletQueryHandler> logger)
    {
        _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PromotorWalletDto>> Handle(
        GetPromotorWalletQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validation
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<PromotorWalletDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Resolve Promotor from UserId
            var promotor = await _walletService.GetPromotorByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<PromotorWalletDto>(
                    "No tienes un perfil de promotor",
                    ServiceResponseMessageType.NotFound_Promotor);
            }

            // 3. Get Wallet (with moneda nombre)
            var (wallet, monedaNombre) = await _walletService.GetWalletConMonedaAsync(
                promotor.PromotorId, cancellationToken);
            if (wallet == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<PromotorWalletDto>(
                    "No tienes un wallet asignado",
                    ServiceResponseMessageType.NotFound_Wallet);
            }

            // 4. Map Entity -> DTO
            var dto = _mapper.Map<PromotorWalletDto>(wallet);

            // 5. Assign MinimoRetiro from configuration (does NOT exist on entity)
            dto.MinimoRetiro = _configuration.GetValue<decimal>("Crowdpromotion:MinimoRetiro", 10.0m);

            // 6. Return success
            return new ServiceResponse<PromotorWalletDto> { Data = dto };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo wallet del promotor para UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PromotorWalletDto>(
                "Error inesperado al obtener el wallet",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
```

---

### 3.2 GetWalletTransaccionesQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Queries/GetWalletTransaccionesQuery.cs`

**Contiene:** Query + Handler en el mismo archivo (REGLA 2)

**Tipo:** Query paginada con filtros (solo lectura)

#### 3.2.1 Query

| Propiedad | Tipo | Requerido | Default | Origen | Validacion |
|-----------|------|-----------|---------|--------|------------|
| `UserId` | `string` | Si | - | JWT claim `sub` | NotEmpty |
| `EsCredito` | `bool?` | No | null | Query param | - |
| `EstadoTransaccionId` | `int?` | No | null | Query param | InclusiveBetween(1,4) si no null |
| `FechaDesde` | `DateTime?` | No | null | Query param | <= FechaHasta si ambas presentes |
| `FechaHasta` | `DateTime?` | No | null | Query param | >= FechaDesde si ambas presentes |
| `Page` | `int` | No | 1 | Query param | GreaterThanOrEqualTo(1) |
| `PageSize` | `int` | No | 10 | Query param | InclusiveBetween(1, 50) |

**Implementa:** `IRequest<ServiceResponse<WalletTransaccionesPagedDto>>`

#### 3.2.2 Handler

**Clase:** `GetWalletTransaccionesQueryHandler`

**Dependencias inyectadas:**

| Campo | Tipo | Proposito |
|-------|------|-----------|
| `_walletService` | `IPromotorWalletService` | Resolver promotor, wallet y obtener transacciones paginadas |
| `_mapper` | `IMapper` | Mapear `List<PromotorWalletTransaccion>` -> `List<WalletTransaccionItemDto>` |
| `_validator` | `IValidator<GetWalletTransaccionesQuery>` | Validar parametros de paginado y fechas |
| `_logger` | `ILogger<GetWalletTransaccionesQueryHandler>` | Logging de errores |

**CRITICO:** Todas las dependencias con `?? throw new ArgumentNullException(nameof(...))` en el constructor.

**Flujo del Handle:**

```
1. Validar request con IValidator<GetWalletTransaccionesQuery>
   └─> !IsValid => return ServiceResponse con validationResult.GetServiceResponseMessages()

2. Resolver promotor via _walletService.GetPromotorByUserIdAsync(request.UserId, ct)
   └─> null => return NotFoundServiceResponse con NotFound_Promotor ("2015")
              mensaje: "No tienes un perfil de promotor"

3. Obtener wallet via _walletService.GetWalletConMonedaAsync(promotor.PromotorId, ct)
   └─> wallet == null => return NotFoundServiceResponse con NotFound_Wallet ("2030")
                         mensaje: "No tienes un wallet asignado"

4. Obtener transacciones paginadas via _walletService.GetTransaccionesPagedAsync(
       walletId: wallet.Id,
       esCredito: request.EsCredito,
       estadoTransaccionId: request.EstadoTransaccionId,
       fechaDesde: request.FechaDesde,
       fechaHasta: request.FechaHasta,
       page: request.Page,
       pageSize: request.PageSize,
       ct: cancellationToken)
   => retorna (IReadOnlyList<PromotorWalletTransaccion> Items, int TotalCount)

5. Mapear entidades -> DTOs via _mapper.Map<List<WalletTransaccionItemDto>>(items)

6. Calcular TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
   NOTA: Si request.PageSize == 0 (defensa), usar 1 para evitar DivisionByZero.

7. Construir WalletTransaccionesPagedDto:
   {
       Items = itemDtos,
       TotalCount = totalCount,
       Page = request.Page,
       PageSize = request.PageSize,
       TotalPages = totalPages
   }

8. Retornar ServiceResponse<WalletTransaccionesPagedDto> { Data = pagedDto }

9. try-catch: capturar Exception, LogError, retornar InternalServerErrorServiceResponse con Internal_UnexpectedError ("5000")
```

**Codigo del archivo completo:**

```csharp
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;

public class GetWalletTransaccionesQuery : IRequest<ServiceResponse<WalletTransaccionesPagedDto>>
{
    /// <summary>UserId extraido del claim 'sub' del JWT.</summary>
    public string UserId { get; set; } = null!;

    /// <summary>Filtro por tipo: true=creditos/ingresos, false=debitos/retiros. Null = todos.</summary>
    public bool? EsCredito { get; set; }

    /// <summary>Filtro por EstadoTransaccionId (1=Pendiente, 2=Procesada, 3=Pagada, 4=Cancelada). Null = todos.</summary>
    public int? EstadoTransaccionId { get; set; }

    /// <summary>Fecha de inicio del rango (inclusive). Null = sin limite inferior.</summary>
    public DateTime? FechaDesde { get; set; }

    /// <summary>Fecha de fin del rango (inclusive). Null = sin limite superior.</summary>
    public DateTime? FechaHasta { get; set; }

    /// <summary>Numero de pagina (1-based). Default: 1.</summary>
    public int Page { get; set; } = 1;

    /// <summary>Tamano de pagina. Default: 10. Maximo: 50.</summary>
    public int PageSize { get; set; } = 10;
}

public class GetWalletTransaccionesQueryHandler
    : IRequestHandler<GetWalletTransaccionesQuery, ServiceResponse<WalletTransaccionesPagedDto>>
{
    private readonly IPromotorWalletService _walletService;
    private readonly IMapper _mapper;
    private readonly IValidator<GetWalletTransaccionesQuery> _validator;
    private readonly ILogger<GetWalletTransaccionesQueryHandler> _logger;

    public GetWalletTransaccionesQueryHandler(
        IPromotorWalletService walletService,
        IMapper mapper,
        IValidator<GetWalletTransaccionesQuery> validator,
        ILogger<GetWalletTransaccionesQueryHandler> logger)
    {
        _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<WalletTransaccionesPagedDto>> Handle(
        GetWalletTransaccionesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validation
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<WalletTransaccionesPagedDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Resolve Promotor from UserId
            var promotor = await _walletService.GetPromotorByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<WalletTransaccionesPagedDto>(
                    "No tienes un perfil de promotor",
                    ServiceResponseMessageType.NotFound_Promotor);
            }

            // 3. Get Wallet (only to obtain wallet.Id; moneda name not needed here)
            var (wallet, _) = await _walletService.GetWalletConMonedaAsync(
                promotor.PromotorId, cancellationToken);
            if (wallet == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<WalletTransaccionesPagedDto>(
                    "No tienes un wallet asignado",
                    ServiceResponseMessageType.NotFound_Wallet);
            }

            // 4. Get paged transactions from service
            var (items, totalCount) = await _walletService.GetTransaccionesPagedAsync(
                walletId: wallet.Id,
                esCredito: request.EsCredito,
                estadoTransaccionId: request.EstadoTransaccionId,
                fechaDesde: request.FechaDesde,
                fechaHasta: request.FechaHasta,
                page: request.Page,
                pageSize: request.PageSize,
                ct: cancellationToken);

            // 5. Map entities -> DTOs
            var itemDtos = _mapper.Map<List<WalletTransaccionItemDto>>(items);

            // 6. Calculate total pages (guard against PageSize == 0)
            var effectivePageSize = request.PageSize > 0 ? request.PageSize : 1;
            var totalPages = (int)Math.Ceiling((double)totalCount / effectivePageSize);

            // 7. Build paged response
            var pagedDto = new WalletTransaccionesPagedDto
            {
                Items = itemDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };

            // 8. Return success
            return new ServiceResponse<WalletTransaccionesPagedDto> { Data = pagedDto };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error obteniendo transacciones del wallet para UserId {UserId}, Page {Page}, PageSize {PageSize}",
                request.UserId, request.Page, request.PageSize);
            return ValidateExtensions.InternalServerErrorServiceResponse<WalletTransaccionesPagedDto>(
                "Error inesperado al obtener las transacciones del wallet",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
```

---

## 4. Commands

### 4.1 SolicitarCobroCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Commands/SolicitarCobroCommand.cs`

**Contiene:** Command + Handler en el mismo archivo (REGLA 2)

**Tipo:** Command (modifica estado: crea transaccion de debito + decrementa saldo con RowVersion)

#### 4.1.1 Command

| Propiedad | Tipo | Requerido | Origen | Validacion FluentValidation |
|-----------|------|-----------|--------|----------------------------|
| `UserId` | `string` | Si | JWT claim `sub` | NotEmpty -> Validation_Required (1001) |
| `Importe` | `decimal` | Si | Request body | GreaterThan(0m) -> Validation_RangeOutOfBounds (1021) |
| `Descripcion` | `string?` | No | Request body | MaximumLength(500) si no null -> Validation_MaxLength (1002) |

**Implementa:** `IRequest<ServiceResponse<SolicitarCobroResponseDto>>`

**NOTA ARQUITECTONICA:** Las validaciones de negocio (saldo insuficiente, saldo bajo minimo, cobro concurrente) NO estan en el Validator. Esas reglas requieren consultar la BD y se implementan en `IPromotorWalletService.SolicitarCobroAsync`. El Handler mapea el `CobroError` resultante al `ServiceResponseMessageType` correcto.

#### 4.1.2 Handler

**Clase:** `SolicitarCobroCommandHandler`

**Dependencias inyectadas:**

| Campo | Tipo | Proposito |
|-------|------|-----------|
| `_walletService` | `IPromotorWalletService` | Resolver promotor y ejecutar cobro atomico |
| `_validator` | `IValidator<SolicitarCobroCommand>` | Validar campos del command (formato, no negocio) |
| `_logger` | `ILogger<SolicitarCobroCommandHandler>` | Logging de errores |

**NOTA:** Este Handler NO inyecta `IMapper`. La respuesta `SolicitarCobroResponseDto` se construye directamente desde `SolicitarCobroResult` porque el record ya tiene los campos listos en el formato correcto. No hay mapeo AutoMapper para Command -> Response en este caso.

**CRITICO:** Todas las dependencias con `?? throw new ArgumentNullException(nameof(...))` en el constructor.

**Flujo del Handle:**

```
1. Validar request con IValidator<SolicitarCobroCommand>
   └─> !IsValid => return ServiceResponse con validationResult.GetServiceResponseMessages()

2. Resolver promotor via _walletService.GetPromotorByUserIdAsync(request.UserId, ct)
   └─> null => return NotFoundServiceResponse con NotFound_Promotor ("2015")
              mensaje: "No tienes un perfil de promotor"

3. Ejecutar cobro atomico via _walletService.SolicitarCobroAsync(
       promotorId: promotor.PromotorId,
       importe: request.Importe,
       descripcion: request.Descripcion,
       ct: cancellationToken)
   => retorna (SolicitarCobroResult? Result, CobroError Error)

4. Mapear CobroError a ServiceResponse segun la siguiente tabla:
   | CobroError.WalletNoEncontrado | NotFoundServiceResponse   | NotFound_Wallet ("2030")                 | mensaje: "No tienes un wallet asignado" |
   | CobroError.SaldoBajoMinimo    | ConflictServiceResponse   | BusinessRule_SaldoBajoMinimoRetiro ("4041") | mensaje: "Saldo disponible inferior al minimo de retiro" |
   | CobroError.SaldoInsuficiente  | ConflictServiceResponse   | BusinessRule_SaldoInsuficiente ("4040")  | mensaje: "Saldo insuficiente para el importe solicitado" |
   | CobroError.CobroConcurrente   | ConflictServiceResponse   | BusinessRule_CobroConcurrente ("4042")   | mensaje: "Ya tienes una solicitud de cobro pendiente" |

5. Si CobroError.None => construir SolicitarCobroResponseDto desde cobroResult:
   {
       TransaccionId = result.TransaccionId,
       Importe = result.Importe,
       MonedaNombre = result.MonedaNombre,
       EstadoTransaccionNombre = "Pendiente",   // SIEMPRE Pendiente en MVP
       SaldoRestante = result.SaldoRestante,
       FechaCreacion = result.FechaCreacion
   }

6. Retornar ServiceResponse<SolicitarCobroResponseDto>:
   {
       Data = responseDto,
       Messages = new List<ServiceResponseMessage>
       {
           new() { Message = "Solicitud de cobro registrada", ErrorCode = ServiceResponseMessageType.Created }
       }
   }

7. try-catch: capturar Exception, LogError con {UserId, Importe}, retornar InternalServerErrorServiceResponse con Internal_UnexpectedError ("5000")
```

**Codigo del archivo completo:**

```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Commands;

public class SolicitarCobroCommand : IRequest<ServiceResponse<SolicitarCobroResponseDto>>
{
    /// <summary>UserId extraido del claim 'sub' del JWT.</summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Importe a retirar. Debe ser mayor que 0 y menor o igual al SaldoDisponible.
    /// La validacion de SaldoDisponible se realiza en el Service (regla de negocio).
    /// </summary>
    public decimal Importe { get; set; }

    /// <summary>Descripcion opcional del retiro. Max 500 caracteres.</summary>
    public string? Descripcion { get; set; }
}

public class SolicitarCobroCommandHandler
    : IRequestHandler<SolicitarCobroCommand, ServiceResponse<SolicitarCobroResponseDto>>
{
    private readonly IPromotorWalletService _walletService;
    private readonly IValidator<SolicitarCobroCommand> _validator;
    private readonly ILogger<SolicitarCobroCommandHandler> _logger;

    public SolicitarCobroCommandHandler(
        IPromotorWalletService walletService,
        IValidator<SolicitarCobroCommand> validator,
        ILogger<SolicitarCobroCommandHandler> logger)
    {
        _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<SolicitarCobroResponseDto>> Handle(
        SolicitarCobroCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validation (format/structure only - business rules are in Service)
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<SolicitarCobroResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Resolve Promotor from UserId
            var promotor = await _walletService.GetPromotorByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<SolicitarCobroResponseDto>(
                    "No tienes un perfil de promotor",
                    ServiceResponseMessageType.NotFound_Promotor);
            }

            // 3. Execute atomic cobro (service handles: wallet check, business validations,
            //    optimistic concurrency with RowVersion, and DB transaction)
            var (cobroResult, cobroError) = await _walletService.SolicitarCobroAsync(
                promotorId: promotor.PromotorId,
                importe: request.Importe,
                descripcion: request.Descripcion,
                ct: cancellationToken);

            // 4. Map CobroError to ServiceResponse
            if (cobroError != CobroError.None)
            {
                return cobroError switch
                {
                    CobroError.WalletNoEncontrado => ValidateExtensions.NotFoundServiceResponse<SolicitarCobroResponseDto>(
                        "No tienes un wallet asignado",
                        ServiceResponseMessageType.NotFound_Wallet),

                    CobroError.SaldoBajoMinimo => ValidateExtensions.ConflictServiceResponse<SolicitarCobroResponseDto>(
                        "El saldo disponible es inferior al minimo de retiro",
                        ServiceResponseMessageType.BusinessRule_SaldoBajoMinimoRetiro),

                    CobroError.SaldoInsuficiente => ValidateExtensions.ConflictServiceResponse<SolicitarCobroResponseDto>(
                        "Saldo insuficiente para el importe solicitado",
                        ServiceResponseMessageType.BusinessRule_SaldoInsuficiente),

                    CobroError.CobroConcurrente => ValidateExtensions.ConflictServiceResponse<SolicitarCobroResponseDto>(
                        "Ya tienes una solicitud de cobro pendiente. Espera a que sea procesada",
                        ServiceResponseMessageType.BusinessRule_CobroConcurrente),

                    _ => ValidateExtensions.InternalServerErrorServiceResponse<SolicitarCobroResponseDto>(
                        "Error inesperado al procesar la solicitud de cobro",
                        ServiceResponseMessageType.Internal_UnexpectedError)
                };
            }

            // 5. Build response DTO from SolicitarCobroResult
            var responseDto = new SolicitarCobroResponseDto
            {
                TransaccionId = cobroResult!.TransaccionId,
                Importe = cobroResult.Importe,
                MonedaNombre = cobroResult.MonedaNombre,
                EstadoTransaccionNombre = "Pendiente",  // Always Pendiente (EstadoTransaccionId = 1) in MVP
                SaldoRestante = cobroResult.SaldoRestante,
                FechaCreacion = cobroResult.FechaCreacion
            };

            // 6. Return success (201 Created)
            return new ServiceResponse<SolicitarCobroResponseDto>
            {
                Data = responseDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Solicitud de cobro registrada",
                        ErrorCode = ServiceResponseMessageType.Created
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error procesando solicitud de cobro para UserId {UserId}, Importe {Importe}",
                request.UserId, request.Importe);
            return ValidateExtensions.InternalServerErrorServiceResponse<SolicitarCobroResponseDto>(
                "Error inesperado al procesar la solicitud de cobro",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
```

---

## 5. Validators

### 5.1 GetPromotorWalletQueryValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Validators/GetPromotorWalletQueryValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Wallet.Validators`

**CRITICO:** Usar `ServiceResponseMessageType.X` constants (NO strings literales)

| Campo | Regla | Mensaje | ErrorCode (Constante) |
|-------|-------|---------|----------------------|
| `UserId` | `NotEmpty()` | "El identificador de usuario es obligatorio" | `ServiceResponseMessageType.Validation_Required` ("1001") |

**Codigo del archivo completo:**

```csharp
using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Validators;

public class GetPromotorWalletQueryValidator : AbstractValidator<GetPromotorWalletQuery>
{
    public GetPromotorWalletQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El identificador de usuario es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
```

---

### 5.2 GetWalletTransaccionesQueryValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Validators/GetWalletTransaccionesQueryValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Wallet.Validators`

| Campo | Regla | Mensaje | ErrorCode (Constante) |
|-------|-------|---------|----------------------|
| `UserId` | `NotEmpty()` | "El identificador de usuario es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `Page` | `GreaterThanOrEqualTo(1)` | "La pagina debe ser mayor o igual a 1" | `ServiceResponseMessageType.Validation_RangeOutOfBounds` |
| `PageSize` | `InclusiveBetween(1, 50)` | "El tamano de pagina debe estar entre 1 y 50" | `ServiceResponseMessageType.Validation_RangeOutOfBounds` |
| `EstadoTransaccionId` | `InclusiveBetween(1, 4)` cuando HasValue | "El estado de transaccion debe ser entre 1 y 4" | `ServiceResponseMessageType.Validation_RangeOutOfBounds` |
| `FechaDesde` | `LessThanOrEqualTo(FechaHasta)` cuando ambas presentes | "La fecha de inicio no puede ser posterior a la fecha de fin" | `ServiceResponseMessageType.Validation_FechaRangoInvalido` |

**Codigo del archivo completo:**

```csharp
using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Validators;

public class GetWalletTransaccionesQueryValidator : AbstractValidator<GetWalletTransaccionesQuery>
{
    public GetWalletTransaccionesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El identificador de usuario es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("La pagina debe ser mayor o igual a 1")
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("El tamano de pagina debe estar entre 1 y 50")
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);

        RuleFor(x => x.EstadoTransaccionId)
            .InclusiveBetween(1, 4)
            .WithMessage("El estado de transaccion debe ser entre 1 y 4")
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds)
            .When(x => x.EstadoTransaccionId.HasValue);

        RuleFor(x => x.FechaDesde)
            .LessThanOrEqualTo(x => x.FechaHasta!.Value)
            .WithMessage("La fecha de inicio no puede ser posterior a la fecha de fin")
            .WithErrorCode(ServiceResponseMessageType.Validation_FechaRangoInvalido)
            .When(x => x.FechaDesde.HasValue && x.FechaHasta.HasValue);
    }
}
```

---

### 5.3 SolicitarCobroCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Validators/SolicitarCobroCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Wallet.Validators`

**NOTA CRITICA:** Este validator valida SOLO formato/estructura. Las reglas de negocio (importe vs SaldoDisponible, SaldoDisponible vs MinimoRetiro, cobros pendientes existentes) son responsabilidad de `IPromotorWalletService.SolicitarCobroAsync`. El validator NO inyecta ningun servicio (no necesita acceder a BD).

| Campo | Regla | Mensaje | ErrorCode (Constante) |
|-------|-------|---------|----------------------|
| `UserId` | `NotEmpty()` | "El identificador de usuario es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `Importe` | `GreaterThan(0m)` | "El importe debe ser mayor que cero" | `ServiceResponseMessageType.Validation_RangeOutOfBounds` |
| `Descripcion` | `MaximumLength(500)` cuando `!= null` | "La descripcion no puede superar los 500 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |

**Codigo del archivo completo:**

```csharp
using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Commands;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Validators;

public class SolicitarCobroCommandValidator : AbstractValidator<SolicitarCobroCommand>
{
    public SolicitarCobroCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El identificador de usuario es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Importe)
            .GreaterThan(0m)
            .WithMessage("El importe debe ser mayor que cero")
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);

        RuleFor(x => x.Descripcion)
            .MaximumLength(500)
            .WithMessage("La descripcion no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => x.Descripcion != null);
    }
}
```

---

## 6. AutoMapper Profile

### 6.1 PromotorWalletProfile

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Mapping/PromotorWalletProfile.cs`

**Patron:** Un Profile por entidad (no agregar al `PromoEventoProfile.cs` existente).

**Mappings configurados:**

**PromotorWallet -> PromotorWalletDto:**

| Campo Entidad | Campo DTO | Tipo Mapping | Nota |
|---------------|-----------|--------------|------|
| `Id` | `WalletId` | `ForMember` con `MapFrom(src => src.Id)` | Nombre diferente |
| `MonedaId` | `MonedaId` | Automatico | Mismo nombre |
| `Moneda.Nombre` (nav.) | `MonedaNombre` | `ForMember` con `MapFrom(src => src.Moneda.Nombre)` | Requiere `.Include(x => x.Moneda)` en repositorio |
| `SaldoDisponible` | `SaldoDisponible` | Automatico | - |
| `SaldoPendiente` | `SaldoPendiente` | Automatico | - |
| `TotalGanado` | `TotalGanado` | Automatico | - |
| `TotalRetirado` | `TotalRetirado` | Automatico | - |
| *(no existe)* | `MinimoRetiro` | `Ignore()` | Se asigna en el Handler desde IConfiguration |

**PromotorWalletTransaccion -> WalletTransaccionItemDto:**

| Campo Entidad | Campo DTO | Tipo Mapping | Nota |
|---------------|-----------|--------------|------|
| `Id` | `Id` | Automatico | - |
| `EsCredito` | `EsCredito` | Automatico | Campo nuevo de dominio (US-CP-06) |
| `Importe` | `Importe` | Automatico | - |
| `Descripcion` | `Descripcion` | Automatico | Campo nuevo de dominio (US-CP-06) |
| `Concepto` | `Concepto` | Automatico | - |
| `EstadoTransaccionId` | `EstadoTransaccionId` | Automatico | - |
| `EstadoTransaccion.Nombre` (nav.) | `EstadoTransaccionNombre` | `ForMember` con `MapFrom` | Requiere `.Include(x => x.EstadoTransaccion)` |
| `TipoRewardId` | `TipoRewardId` | Automatico | Nullable |
| `TipoReward.Nombre` (nav.) | `TipoRewardNombre` | `ForMember` con null-check | Requiere `.Include(x => x.TipoReward)` |
| `PromoEventoId` | `PromoEventoId` | Automatico | Campo nuevo de dominio (US-CP-06) |
| `FechaCreacion` | `FechaCreacion` | Automatico | - |
| `FechaProcesado` | `FechaProcesado` | Automatico | Nullable |

**NOTA sobre navigation includes:** El repositorio `PromotorWalletTransaccionRepository.GetPagedByWalletIdAsync` debe hacer `Include(x => x.EstadoTransaccion)` y `Include(x => x.TipoReward)` para que AutoMapper pueda acceder a los nombres. Sin los includes, AutoMapper lanzaria `NullReferenceException` al acceder a `.EstadoTransaccion.Nombre`. El repositorio `IPromotorWalletRepository` (GetWalletConMonedaAsync) debe hacer `Include(x => x.Moneda)`.

**Codigo del archivo completo:**

```csharp
using AutoMapper;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Mapping;

public class PromotorWalletProfile : Profile
{
    public PromotorWalletProfile()
    {
        // PromotorWallet -> PromotorWalletDto
        // NOTE: MinimoRetiro is ignored here - Handler assigns it from IConfiguration
        CreateMap<PromotorWallet, PromotorWalletDto>()
            .ForMember(dest => dest.WalletId,
                       opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MonedaNombre,
                       opt => opt.MapFrom(src => src.Moneda.Nombre))
            .ForMember(dest => dest.MinimoRetiro,
                       opt => opt.Ignore());

        // PromotorWalletTransaccion -> WalletTransaccionItemDto
        // NOTE: Repository must Include EstadoTransaccion and TipoReward for navigation mappings
        CreateMap<PromotorWalletTransaccion, WalletTransaccionItemDto>()
            .ForMember(dest => dest.EstadoTransaccionNombre,
                       opt => opt.MapFrom(src => src.EstadoTransaccion.Nombre))
            .ForMember(dest => dest.TipoRewardNombre,
                       opt => opt.MapFrom(src => src.TipoReward != null
                                                 ? src.TipoReward.Nombre
                                                 : null));
    }
}
```

---

## 7. Flujo de Datos Completo

### 7.1 GET /api/crowdpromotion/promotor/wallet

```
HTTP GET /api/crowdpromotion/promotor/wallet
  [Header] Authorization: Bearer <JWT>
     |
     v
PromotorController.GetWallet(ct)
  -> Extrae _currentUser.UserId (null => 401 Auth_InvalidToken)
  -> Construye GetPromotorWalletQuery { UserId = userId.ToString() }
  -> _mediator.Send(query, ct)
     |
     v
GetPromotorWalletQueryHandler.Handle(query, ct)
  -> GetPromotorWalletQueryValidator: valida UserId not empty
  -> _walletService.GetPromotorByUserIdAsync(userId, ct)
     [IPromotorService.GetByUserIdAsync via cache "promotor:userid:{userId}"]
     └─> null => 404 NotFound_Promotor ("2015")
  -> _walletService.GetWalletConMonedaAsync(promotorId, ct)
     [IPromotorWalletRepository.GetByPromotorIdAndMonedaAsync con Include(Moneda)]
     [cache "promotorwallet:{promotorId}:moneda:1"]
     └─> null => 404 NotFound_Wallet ("2030")
  -> _mapper.Map<PromotorWalletDto>(wallet)
  -> dto.MinimoRetiro = _configuration["Crowdpromotion:MinimoRetiro"] (default 10.0m)
  -> ServiceResponse<PromotorWalletDto> { Data = dto }
     |
     v
FromServiceResponse(result)
  -> 200 OK con PromotorWalletDto
```

### 7.2 GET /api/crowdpromotion/promotor/wallet/transacciones

```
HTTP GET /api/crowdpromotion/promotor/wallet/transacciones?page=1&pageSize=10&esCredito=true
  [Header] Authorization: Bearer <JWT>
     |
     v
PromotorController.GetWalletTransacciones(esCredito, estadoTransaccionId, fechaDesde, fechaHasta, page, pageSize, ct)
  -> Extrae _currentUser.UserId (null => 401 Auth_InvalidToken)
  -> Parsea fechaDesde/fechaHasta string -> DateTime? (formato invalido => 400 Validation_FechaRangoInvalido)
  -> Construye GetWalletTransaccionesQuery { UserId, EsCredito, ..., Page, PageSize }
  -> _mediator.Send(query, ct)
     |
     v
GetWalletTransaccionesQueryHandler.Handle(query, ct)
  -> GetWalletTransaccionesQueryValidator:
     - UserId not empty
     - Page >= 1
     - PageSize in [1, 50]
     - EstadoTransaccionId in [1, 4] si HasValue
     - FechaDesde <= FechaHasta si ambas presentes
  -> _walletService.GetPromotorByUserIdAsync(userId, ct)
     └─> null => 404 NotFound_Promotor ("2015")
  -> _walletService.GetWalletConMonedaAsync(promotorId, ct)
     └─> null => 404 NotFound_Wallet ("2030")
  -> _walletService.GetTransaccionesPagedAsync(walletId, filtros..., page, pageSize, ct)
     [IPromotorWalletTransaccionRepository.GetPagedByWalletIdAsync]
     [2 queries SQL: COUNT(*) + SELECT paginado con ORDER BY FechaCreacion DESC]
     [Include: EstadoTransaccion, TipoReward]
     => (IReadOnlyList<PromotorWalletTransaccion> items, int totalCount)
  -> _mapper.Map<List<WalletTransaccionItemDto>>(items)
  -> totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
  -> WalletTransaccionesPagedDto { Items, TotalCount, Page, PageSize, TotalPages }
  -> ServiceResponse<WalletTransaccionesPagedDto> { Data = pagedDto }
     |
     v
FromServiceResponse(result)
  -> 200 OK con WalletTransaccionesPagedDto
```

### 7.3 POST /api/crowdpromotion/promotor/wallet/cobro

```
HTTP POST /api/crowdpromotion/promotor/wallet/cobro
  [Header] Authorization: Bearer <JWT>
  [Body]   { "importe": 50.00, "descripcion": "Retiro mensual" }
     |
     v
PromotorController.SolicitarCobro(request, ct)
  -> Extrae _currentUser.UserId (null => 401 Auth_InvalidToken)
  -> Construye SolicitarCobroCommand { UserId, Importe, Descripcion }
  -> _mediator.Send(command, ct)
     |
     v
SolicitarCobroCommandHandler.Handle(command, ct)
  -> SolicitarCobroCommandValidator:
     - UserId not empty
     - Importe > 0m
     - Descripcion MaxLength(500) si no null
  -> _walletService.GetPromotorByUserIdAsync(userId, ct)
     └─> null => 404 NotFound_Promotor ("2015")
  -> _walletService.SolicitarCobroAsync(promotorId, importe, descripcion, ct)
     |
     v (dentro del service - logica de negocio)
     BEGIN TRANSACTION (CrowdpromotionContext)
       -> GetByPromotorIdAndMonedaForUpdateAsync(promotorId, monedaId=1, ct)  [CON tracking]
          └─> null => retorna (null, CobroError.WalletNoEncontrado)
       -> _configuration["Crowdpromotion:MinimoRetiro"] (default 10.0m)
       -> Validar wallet.SaldoDisponible >= minimoRetiro
          └─> no => retorna (null, CobroError.SaldoBajoMinimo)
       -> Validar importe <= wallet.SaldoDisponible
          └─> no => retorna (null, CobroError.SaldoInsuficiente)
       -> HasPendienteByWalletIdAsync(walletId, ct)
          └─> true => retorna (null, CobroError.CobroConcurrente)
       -> Crear PromotorWalletTransaccion { EsCredito=false, EstadoId=1, Importe, Descripcion, ... }
       -> _transaccionRepository.AddAsync(transaccion, ct)  [sin SaveChanges]
       -> wallet.SaldoDisponible -= importe; wallet.TotalRetirado += importe; wallet.FechaActualizacion = UtcNow
       -> _walletRepository.UpdateSaldoAsync(wallet, ct)    [sin SaveChanges]
       -> _context.SaveChangesAsync(ct)
          [EF genera: UPDATE PromotorWallet SET ... WHERE Id=? AND RowVersion=?]
          [DbUpdateConcurrencyException si RowVersion desactualizado]
          └─> DbUpdateConcurrencyException => RollbackAsync, retorna (null, CobroError.CobroConcurrente)
       -> CommitAsync(ct)
       -> retorna (SolicitarCobroResult { TransaccionId, Importe, MonedaNombre, SaldoRestante, FechaCreacion }, CobroError.None)
     END TRANSACTION
     |
     v (de vuelta en el Handler)
  -> Mapear CobroError => ServiceResponse adecuado:
     - WalletNoEncontrado => 404 NotFound_Wallet ("2030")
     - SaldoBajoMinimo    => 409 BusinessRule_SaldoBajoMinimoRetiro ("4041")
     - SaldoInsuficiente  => 409 BusinessRule_SaldoInsuficiente ("4040")
     - CobroConcurrente   => 409 BusinessRule_CobroConcurrente ("4042")
  -> Si None => SolicitarCobroResponseDto { TransaccionId, Importe, MonedaNombre, "Pendiente", SaldoRestante, FechaCreacion }
  -> ServiceResponse { Data = responseDto, Messages = [{ Created ("0001") }] }
     |
     v
result.IsSuccess => CreatedAtAction(nameof(GetWallet), null, result)
                 => 201 Created con SolicitarCobroResponseDto
```

---

## 8. Manejo de Errores por Endpoint

### 8.1 GET /promotor/wallet

| Escenario | Donde se detecta | ErrorCode | HTTP |
|-----------|-----------------|-----------|------|
| Token ausente/invalido | Controller | `Auth_InvalidToken` ("3004") | 401 |
| UserId vacio (guard) | Validator | `Validation_Required` ("1001") | 400 |
| Promotor no existe | Handler via Service | `NotFound_Promotor` ("2015") | 404 |
| Wallet no existe | Handler via Service | `NotFound_Wallet` ("2030") | 404 |
| Error inesperado | Handler try-catch | `Internal_UnexpectedError` ("5000") | 500 |

### 8.2 GET /promotor/wallet/transacciones

| Escenario | Donde se detecta | ErrorCode | HTTP |
|-----------|-----------------|-----------|------|
| Token ausente/invalido | Controller | `Auth_InvalidToken` ("3004") | 401 |
| Formato fecha invalido | Controller | `Validation_FechaRangoInvalido` ("1036") | 400 |
| UserId vacio | Validator | `Validation_Required` ("1001") | 400 |
| page < 1 | Validator | `Validation_RangeOutOfBounds` ("1021") | 400 |
| pageSize fuera de [1,50] | Validator | `Validation_RangeOutOfBounds` ("1021") | 400 |
| estadoTransaccionId fuera de [1,4] | Validator | `Validation_RangeOutOfBounds` ("1021") | 400 |
| fechaDesde > fechaHasta | Validator | `Validation_FechaRangoInvalido` ("1036") | 400 |
| Promotor no existe | Handler via Service | `NotFound_Promotor` ("2015") | 404 |
| Wallet no existe | Handler via Service | `NotFound_Wallet` ("2030") | 404 |
| Error inesperado | Handler try-catch | `Internal_UnexpectedError` ("5000") | 500 |

### 8.3 POST /promotor/wallet/cobro

| Escenario | Donde se detecta | ErrorCode | HTTP |
|-----------|-----------------|-----------|------|
| Token ausente/invalido | Controller | `Auth_InvalidToken` ("3004") | 401 |
| Importe ausente/cero | Validator | `Validation_Required` ("1001") o `Validation_RangeOutOfBounds` ("1021") | 400 |
| Importe <= 0 | Validator | `Validation_RangeOutOfBounds` ("1021") | 400 |
| Descripcion > 500 chars | Validator | `Validation_MaxLength` ("1002") | 400 |
| Promotor no existe | Handler via Service | `NotFound_Promotor` ("2015") | 404 |
| Wallet no existe | Service -> CobroError | `NotFound_Wallet` ("2030") | 404 |
| SaldoDisponible < MinimoRetiro | Service -> CobroError | `BusinessRule_SaldoBajoMinimoRetiro` ("4041") | 409 |
| Importe > SaldoDisponible | Service -> CobroError | `BusinessRule_SaldoInsuficiente` ("4040") | 409 |
| Solicitud pendiente existente | Service -> CobroError | `BusinessRule_CobroConcurrente` ("4042") | 409 |
| Conflicto RowVersion (concurrencia) | Service DbUpdateConcurrencyException | `BusinessRule_CobroConcurrente` ("4042") | 409 |
| Error inesperado | Handler try-catch | `Internal_UnexpectedError` ("5000") | 500 |

---

## 9. Diagrama de Dependencias entre Clases

```
PromotorController
  |-- GetWallet()
  |     └─> GetPromotorWalletQuery
  |               └─> GetPromotorWalletQueryHandler
  |                     |-- IPromotorWalletService
  |                     |     |-- GetPromotorByUserIdAsync()  [delega a IPromotorService o repositorio]
  |                     |     └── GetWalletConMonedaAsync()   [IPromotorWalletRepository + cache]
  |                     |-- IMapper
  |                     |     └── PromotorWalletProfile
  |                     |           └── PromotorWallet -> PromotorWalletDto
  |                     |-- IValidator<GetPromotorWalletQuery>
  |                     |     └── GetPromotorWalletQueryValidator
  |                     |-- IConfiguration  ["Crowdpromotion:MinimoRetiro"]
  |                     └── ILogger<GetPromotorWalletQueryHandler>
  |
  |-- GetWalletTransacciones()
  |     └─> GetWalletTransaccionesQuery
  |               └─> GetWalletTransaccionesQueryHandler
  |                     |-- IPromotorWalletService
  |                     |     |-- GetPromotorByUserIdAsync()
  |                     |     |-- GetWalletConMonedaAsync()
  |                     |     └── GetTransaccionesPagedAsync()  [IPromotorWalletTransaccionRepository]
  |                     |-- IMapper
  |                     |     └── PromotorWalletProfile
  |                     |           └── PromotorWalletTransaccion -> WalletTransaccionItemDto
  |                     |-- IValidator<GetWalletTransaccionesQuery>
  |                     |     └── GetWalletTransaccionesQueryValidator
  |                     └── ILogger<GetWalletTransaccionesQueryHandler>
  |
  └── SolicitarCobro()
        └─> SolicitarCobroCommand
                  └─> SolicitarCobroCommandHandler
                        |-- IPromotorWalletService
                        |     |-- GetPromotorByUserIdAsync()
                        |     └── SolicitarCobroAsync()
                        |           |-- IPromotorWalletRepository.GetByPromotorIdAndMonedaForUpdateAsync()  [con tracking]
                        |           |-- IConfiguration ["Crowdpromotion:MinimoRetiro"]
                        |           |-- IPromotorWalletTransaccionRepository.HasPendienteByWalletIdAsync()
                        |           |-- IPromotorWalletTransaccionRepository.AddAsync()   [sin SaveChanges]
                        |           |-- IPromotorWalletRepository.UpdateSaldoAsync()      [sin SaveChanges]
                        |           |-- CrowdpromotionContext.SaveChangesAsync()           [RowVersion check]
                        |           └── CrowdpromotionContext.CommitAsync()
                        |           Retorna: (SolicitarCobroResult?, CobroError)
                        |-- IValidator<SolicitarCobroCommand>
                        |     └── SolicitarCobroCommandValidator
                        └── ILogger<SolicitarCobroCommandHandler>


CobroError (enum definido en IPromotorWalletService.cs):
  None = 0
  WalletNoEncontrado = 1  -> Handler mapea a NotFound_Wallet ("2030")
  SaldoBajoMinimo = 2     -> Handler mapea a BusinessRule_SaldoBajoMinimoRetiro ("4041")
  SaldoInsuficiente = 3   -> Handler mapea a BusinessRule_SaldoInsuficiente ("4040")
  CobroConcurrente = 4    -> Handler mapea a BusinessRule_CobroConcurrente ("4042")

SolicitarCobroResult (record definido en IPromotorWalletService.cs):
  TransaccionId: Guid
  Importe: decimal
  MonedaNombre: string
  SaldoRestante: decimal
  FechaCreacion: DateTime
```

---

## 10. Constantes ServiceResponseMessageType Requeridas

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

**Accion:** MODIFICAR - agregar al final de la clase existente.

Constantes existentes usadas por esta feature (ya en el archivo):
- `Validation_Required = "1001"` - ya existe
- `Validation_MaxLength = "1002"` - ya existe
- `Validation_RangeOutOfBounds = "1021"` - ya existe
- `Validation_FechaRangoInvalido = "1036"` - ya existe
- `NotFound_Promotor = "2015"` - ya existe
- `Auth_InvalidToken = "3004"` - ya existe
- `Internal_UnexpectedError = "5000"` - ya existe
- `Created = "0001"` - ya existe

Constantes NUEVAS a agregar (aun no en el archivo):

```csharp
// Wallet - US-CP-06 (NotFound 2030-2039)
public const string NotFound_Wallet = "2030";

// Wallet - US-CP-06 (Business Rules 4040-4049)
public const string BusinessRule_SaldoInsuficiente = "4040";
public const string BusinessRule_SaldoBajoMinimoRetiro = "4041";
public const string BusinessRule_CobroConcurrente = "4042";
```

---

## 11. Resolucion de PromotorId desde UserId

Los tres Handlers necesitan resolver el `PromotorId` a partir del `UserId` del JWT. La solucion propuesta en `hexagonal-architecture.md` es que `IPromotorWalletService` exponga `GetPromotorByUserIdAsync`, que internamente puede:

**Opcion A (recomendada):** Inyectar `IPromotorService` dentro de `PromotorWalletService` y delegar:
```csharp
// En PromotorWalletService.cs
public async Task<Promotor?> GetPromotorByUserIdAsync(string userId, CancellationToken ct)
    => await _promotorService.GetByUserIdAsync(userId, ct);
```

**Opcion B:** Inyectar `IPromotorRepository` directamente en `PromotorWalletService` (evita dependencia circular).

La decision queda para el implementador. Lo importante para los Handlers: siempre llaman `_walletService.GetPromotorByUserIdAsync(userId, ct)` y el resultado es `Promotor?`.

**NOTA sobre el tipo retornado:** `GetPromotorByUserIdAsync` retorna la entidad `Promotor` (no un DTO). De ella, el Handler extrae `promotor.PromotorId` (el strongly-typed ID). Esta es la unica propiedad que los Handlers usan del objeto `Promotor`.

---

## 12. Patrones Arquitectonicos Clave

### 12.1 Separacion Validator / Handler / Service

```
Validator (FluentValidation):
  - Valida formato y estructura (campos requeridos, rangos, longitudes maximas)
  - NO accede a BD
  - NO inyecta servicios (en esta feature)
  - Retorna errores de validacion sincrona

Handler:
  - Orquesta el flujo (Validator -> Service -> Map -> Response)
  - Contiene logica de enrutamiento (interpretacion de CobroError -> ServiceResponse)
  - NO inyecta DbContext
  - NO hace queries a BD directamente

Service:
  - Contiene logica de negocio (validaciones que requieren BD: saldo, minimo, concurrencia)
  - Gestiona transacciones de BD atomicas
  - Retorna entidades o result objects, NO DTOs
  - El Handler hace el mapping de entidades a DTOs via AutoMapper
```

### 12.2 CobroError como Resultado Discriminado

En lugar de lanzar excepciones de control de flujo para los errores de negocio del cobro, el Service retorna un enum `CobroError`. Esto sigue el patron de discriminated union y evita el costo de generar stack traces para flujos de negocio esperados (4040, 4041, 4042 son condiciones normales, no excepciones excepcionales).

El Handler mapea `CobroError` a `ServiceResponseMessageType` en un `switch expression` (idiomatico en C# 8+), manteniendo la logica de presentacion (codigos HTTP, mensajes de error) en la capa de Application y no en Infrastructure.

### 12.3 MinimoRetiro desde IConfiguration

`MinimoRetiro` (10.00 EUR) es configurable via `appsettings.json` (`Crowdpromotion:MinimoRetiro`). El valor se lee en:
- `GetPromotorWalletQuery` Handler: para incluirlo en el DTO de respuesta del wallet.
- `PromotorWalletService.SolicitarCobroAsync`: para validar que el saldo supera el minimo antes de permitir el cobro.

Ambos usan `_configuration.GetValue<decimal>("Crowdpromotion:MinimoRetiro", 10.0m)` con fallback `10.0m`.

### 12.4 Concurrencia Optimista con RowVersion

La entidad `PromotorWallet` tiene un campo `RowVersion` (byte[], configurado con `IsRowVersion()` en Fluent API). Al ejecutar `SaveChangesAsync()`, EF Core genera:

```sql
UPDATE PromotorWallet
SET SaldoDisponible = @new, TotalRetirado = @new, FechaActualizacion = @new
WHERE Id = @id AND RowVersion = @originalRowVersion
```

Si entre el `GetByPromotorIdAndMonedaForUpdateAsync` y el `SaveChangesAsync` otro request modifico el wallet, el `RowVersion` en BD ya no coincide y EF lanza `DbUpdateConcurrencyException`. El service captura esta excepcion especifica, hace rollback, y retorna `(null, CobroError.CobroConcurrente)`. El Handler lo convierte a HTTP 409 con `BusinessRule_CobroConcurrente`.

---

## 13. Checklist

### Commands
- [ ] `SolicitarCobroCommand` implementa `IRequest<ServiceResponse<SolicitarCobroResponseDto>>`
- [ ] Handler en el MISMO archivo que el Command
- [ ] Handler inyecta `IPromotorWalletService`, `IValidator<SolicitarCobroCommand>`, `ILogger` (NO IMapper en este caso)
- [ ] Constructor con `?? throw new ArgumentNullException` para TODAS las dependencias
- [ ] Handler NO inyecta DbContext (la concurrencia se maneja en el Service)
- [ ] Handler mapea `CobroError` a `ServiceResponseMessageType` con switch expression
- [ ] Handler retorna `ServiceResponse` con `ErrorCode = ServiceResponseMessageType.Created` en caso de exito

### Queries
- [ ] `GetPromotorWalletQuery` implementa `IRequest<ServiceResponse<PromotorWalletDto>>`
- [ ] `GetWalletTransaccionesQuery` implementa `IRequest<ServiceResponse<WalletTransaccionesPagedDto>>`
- [ ] Handlers en el MISMO archivo que sus Queries
- [ ] Handlers inyectan `IPromotorWalletService`, `IMapper`, `IValidator<T>`, `ILogger`
- [ ] `GetPromotorWalletQueryHandler` inyecta ADICIONALMENTE `IConfiguration` para MinimoRetiro
- [ ] Constructor con `?? throw new ArgumentNullException` para TODAS las dependencias
- [ ] `dto.MinimoRetiro` se asigna en Handler desde IConfiguration (NO desde entidad ni AutoMapper)
- [ ] `TotalPages` se calcula en Handler: `(int)Math.Ceiling((double)totalCount / pageSize)`

### Validators
- [ ] Validators en carpeta `Validators/` separada
- [ ] Validators usan `ServiceResponseMessageType.X` (NO strings literales)
- [ ] Validators tienen `.WithMessage()` Y `.WithErrorCode()` en TODAS las reglas
- [ ] `SolicitarCobroCommandValidator` NO valida reglas de negocio (saldo, minimo, concurrencia)
- [ ] `GetWalletTransaccionesQueryValidator` valida Page, PageSize, EstadoTransaccionId y rango de fechas

### AutoMapper
- [ ] `PromotorWalletProfile` es un archivo separado (no modificar `PromoEventoProfile.cs`)
- [ ] `PromotorWallet.Id` -> `PromotorWalletDto.WalletId` con ForMember explicito
- [ ] `PromotorWallet.Moneda.Nombre` -> `PromotorWalletDto.MonedaNombre` con ForMember
- [ ] `PromotorWalletDto.MinimoRetiro` ignorado en AutoMapper (Ignore())
- [ ] `PromotorWalletTransaccion.EstadoTransaccion.Nombre` -> `WalletTransaccionItemDto.EstadoTransaccionNombre`
- [ ] `PromotorWalletTransaccion.TipoReward?.Nombre` -> `WalletTransaccionItemDto.TipoRewardNombre` con null-check

### Domain Constants
- [ ] `NotFound_Wallet = "2030"` agregada a `ServiceResponseMessageType.cs`
- [ ] `BusinessRule_SaldoInsuficiente = "4040"` agregada
- [ ] `BusinessRule_SaldoBajoMinimoRetiro = "4041"` agregada
- [ ] `BusinessRule_CobroConcurrente = "4042"` agregada
- [ ] `CobroError` enum definido en `IPromotorWalletService.cs`
- [ ] `SolicitarCobroResult` record definido en `IPromotorWalletService.cs`

### General
- [ ] TODOS los handlers tienen try-catch con LogError y retorno de Internal_UnexpectedError
- [ ] NUNCA se retornan excepciones, SIEMPRE ServiceResponse
- [ ] Los namespaces siguen el patron: `WePlayRises.Crowdpromotion.Application.Features.Wallet.{Queries|Commands|Validators}`
