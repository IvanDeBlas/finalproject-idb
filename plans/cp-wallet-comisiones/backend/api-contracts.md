# Contratos API: cp-wallet-comisiones

**Fecha:** 2026-03-02
**Modulo:** Crowdpromotion
**Feature:** cp-wallet-comisiones (US-CP-06: Wallet de Promotor, Comisiones y Cobros)
**Depende de:** cp-inscripcion-programa (US-CP-03), cp-tracking-metricas (US-CP-05)

---

## 1. Endpoints

| Metodo | Ruta | Tipo | Handler | Descripcion |
|--------|------|------|---------|-------------|
| GET | `/api/crowdpromotion/promotor/wallet` | Query | `GetPromotorWalletQueryHandler` | Resumen del wallet del promotor autenticado |
| GET | `/api/crowdpromotion/promotor/wallet/transacciones` | Query | `GetWalletTransaccionesQueryHandler` | Historial paginado de transacciones con filtros |
| POST | `/api/crowdpromotion/promotor/wallet/cobro` | Command | `SolicitarCobroCommandHandler` | Registrar solicitud de cobro/retiro |

**Nota de routing:** Los tres endpoints se agregan al `PromotorController` existente en
`Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/Controllers/PromotorController.cs`.
La ruta base del controller es `api/crowdpromotion/[controller]` = `api/crowdpromotion/promotor`,
por lo que los metodos usaran rutas relativas `wallet`, `wallet/transacciones` y `wallet/cobro`.

---

## 2. Cambios de Modelo de Dominio Requeridos (Pre-condicion)

Antes de implementar los contratos API, la entidad `PromotorWalletTransaccion` debe recibir
tres campos nuevos. Estos campos son pre-condicion para que los handlers funcionen correctamente.

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromotorWalletTransaccion.cs`

| Campo | Tipo | Nullable | Descripcion |
|-------|------|----------|-------------|
| `EsCredito` | `bool` | No (NOT NULL) | `true` = credito/ingreso (comision acreditada), `false` = debito/retiro |
| `PromoEventoId` | `Guid?` | Si (NULL) | FK a `PromoEvento` que origino este credito; null para retiros |
| `Descripcion` | `string?` | Si (NULL) | Texto legible del origen, max 500 caracteres |

**Migracion EF Core requerida:**
```
Add-Migration AddWalletTransaccionFields -Project WePlayRises.Crowdpromotion.Infra
```

**Valor default para EsCredito en filas existentes:** `true` (las transacciones historicas de
US-CP-04 y US-CP-05 son todas creditos).

---

## 3. Nuevas Constantes ServiceResponseMessageType

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

Agregar dentro de la clase `ServiceResponseMessageType` existente, respetando los rangos libres.
El ultimo codigo `NotFound` en uso es `2022` y el ultimo `BusinessRule` en uso es `4032`.

### 3.1 Constantes a agregar

```csharp
// Wallet - US-CP-06 (NotFound 2030-2039)
public const string NotFound_Wallet = "2030";

// Wallet - US-CP-06 (Business Rules 4040-4049)
public const string BusinessRule_SaldoInsuficiente = "4040";
public const string BusinessRule_SaldoBajoMinimoRetiro = "4041";
public const string BusinessRule_CobroConcurrente = "4042";
```

### 3.2 Tabla de codigos completa para esta feature

| ErrorCode | Constante | Categoria | Descripcion |
|-----------|-----------|-----------|-------------|
| `1001` | `Validation_Required` | Validation (existente) | importe ausente o vacio |
| `1002` | `Validation_MaxLength` | Validation (existente) | descripcion > 500 caracteres |
| `1021` | `Validation_RangeOutOfBounds` | Validation (existente) | importe <= 0, page < 1, pageSize fuera de [1,50] |
| `1036` | `Validation_FechaRangoInvalido` | Validation (existente) | fechaDesde posterior a fechaHasta |
| `2015` | `NotFound_Promotor` | NotFound (existente) | usuario sin perfil de promotor |
| `2030` | `NotFound_Wallet` | NotFound (NUEVO) | promotor sin wallet asignado |
| `3001` | `Auth_Unauthorized` | Auth (existente) | token ausente o invalido |
| `3002` | `Auth_Forbidden` | Auth (existente) | userId del token sin perfil de promotor |
| `4040` | `BusinessRule_SaldoInsuficiente` | BusinessRule (NUEVO) | importe > SaldoDisponible |
| `4041` | `BusinessRule_SaldoBajoMinimoRetiro` | BusinessRule (NUEVO) | SaldoDisponible < MinimoRetiro |
| `4042` | `BusinessRule_CobroConcurrente` | BusinessRule (NUEVO) | ya existe solicitud pendiente del promotor |
| `5000` | `Internal_UnexpectedError` | Internal (existente) | error inesperado del servidor |

---

## 4. Request DTOs (Commands y Queries)

### 4.1 GetPromotorWalletQuery

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Queries/GetPromotorWalletQuery.cs`

Este archivo contiene el Query + Handler en el mismo archivo (REGLA 2 del CQRS).

**Estructura del Query:**

```csharp
/// <summary>
/// Query para obtener el resumen del wallet del promotor autenticado.
/// El PromotorId se resuelve internamente desde el UserId del token JWT.
/// Retorna 404 con ErrorCode 2015 si el usuario no tiene perfil de promotor.
/// Retorna 404 con ErrorCode 2030 si el promotor aun no tiene wallet asignado.
/// </summary>
public class GetPromotorWalletQuery : IRequest<ServiceResponse<PromotorWalletDto>>
{
    /// <summary>
    /// UserId extraido del claim 'sub' del JWT. El handler lo resuelve al PromotorId.
    /// </summary>
    public string UserId { get; set; } = null!;
}
```

**Implementa:** `IRequest<ServiceResponse<PromotorWalletDto>>`

**Propiedades del Query:**

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `UserId` | `string` | Si | JWT claim `sub` | ID del usuario autenticado |

**Handler - dependencias inyectadas:**

| Dependencia | Tipo | Proposito |
|-------------|------|-----------|
| `IPromotorWalletService` | `IPromotorWalletService` | Obtener wallet del promotor |
| `IMapper` | `IMapper` | Mapear `PromotorWallet` -> `PromotorWalletDto` |
| `ILogger<GetPromotorWalletQueryHandler>` | `ILogger<T>` | Logging de errores |

**Flujo del Handler:**

```
1. Resolver PromotorId desde UserId via IPromotorWalletService.GetPromotorByUserIdAsync
   └─> null => retornar ServiceResponse con NotFound_Promotor (2015)
2. Obtener wallet via IPromotorWalletService.GetWalletByPromotorIdAsync(promotorId)
   └─> null => retornar ServiceResponse con NotFound_Wallet (2030)
3. Obtener MinimoRetiro desde IConfiguration["Crowdpromotion:MinimoRetiro"] (default 10.0m)
4. Mapear PromotorWallet -> PromotorWalletDto via _mapper
5. Asignar PromotorWalletDto.MinimoRetiro con el valor de configuracion
6. Retornar ServiceResponse<PromotorWalletDto> con Data poblado
```

---

### 4.2 GetWalletTransaccionesQuery

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Queries/GetWalletTransaccionesQuery.cs`

Este archivo contiene el Query + Handler en el mismo archivo (REGLA 2 del CQRS).

**Estructura del Query:**

```csharp
/// <summary>
/// Query para obtener el historial paginado de transacciones del wallet del promotor autenticado.
/// Soporta filtros opcionales por tipo (EsCredito), estado, y rango de fechas.
/// Ordenado por FechaCreacion DESC (mas reciente primero).
/// </summary>
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
```

**Implementa:** `IRequest<ServiceResponse<WalletTransaccionesPagedDto>>`

**Propiedades del Query:**

| Propiedad | Tipo | Requerido | Default | Origen | Validacion |
|-----------|------|-----------|---------|--------|------------|
| `UserId` | `string` | Si | - | JWT claim `sub` | NotEmpty |
| `EsCredito` | `bool?` | No | null | Query param | - |
| `EstadoTransaccionId` | `int?` | No | null | Query param | InclusiveBetween(1,4) si no es null |
| `FechaDesde` | `DateTime?` | No | null | Query param | <= FechaHasta si ambas presentes |
| `FechaHasta` | `DateTime?` | No | null | Query param | >= FechaDesde si ambas presentes |
| `Page` | `int` | No | 1 | Query param | GreaterThanOrEqualTo(1) |
| `PageSize` | `int` | No | 10 | Query param | InclusiveBetween(1, 50) |

**Handler - dependencias inyectadas:**

| Dependencia | Tipo | Proposito |
|-------------|------|-----------|
| `IPromotorWalletService` | `IPromotorWalletService` | Obtener wallet y transacciones |
| `IValidator<GetWalletTransaccionesQuery>` | `IValidator<T>` | Validar parametros |
| `IMapper` | `IMapper` | Mapear `PromotorWalletTransaccion` -> `WalletTransaccionItemDto` |
| `ILogger<GetWalletTransaccionesQueryHandler>` | `ILogger<T>` | Logging de errores |

**Flujo del Handler:**

```
1. Ejecutar validator -> si invalido retornar ServiceResponse con mensajes de error
2. Resolver PromotorId desde UserId via IPromotorWalletService.GetPromotorByUserIdAsync
   └─> null => retornar ServiceResponse con NotFound_Promotor (2015)
3. Obtener wallet via IPromotorWalletService.GetWalletByPromotorIdAsync(promotorId)
   └─> null => retornar ServiceResponse con NotFound_Wallet (2030)
4. Obtener transacciones paginadas via IPromotorWalletService.GetTransaccionesPagedAsync(
       walletId, EsCredito, EstadoTransaccionId, FechaDesde, FechaHasta, Page, PageSize)
5. Mapear List<PromotorWalletTransaccion> -> List<WalletTransaccionItemDto> via _mapper
6. Construir WalletTransaccionesPagedDto con Items, TotalCount, Page, PageSize, TotalPages
7. Retornar ServiceResponse<WalletTransaccionesPagedDto> con Data poblado
```

---

### 4.3 SolicitarCobroCommand

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Commands/SolicitarCobroCommand.cs`

Este archivo contiene el Command + Handler en el mismo archivo (REGLA 2 del CQRS).

**Estructura del Command:**

```csharp
/// <summary>
/// Command para registrar una solicitud de cobro/retiro desde el wallet del promotor.
/// Crea una PromotorWalletTransaccion con EsCredito=false y EstadoTransaccionId=1 (Pendiente).
/// Decrementa SaldoDisponible en PromotorWallet usando concurrencia optimista (RowVersion).
/// El procesamiento real del pago es manual en MVP.
/// </summary>
public class SolicitarCobroCommand : IRequest<ServiceResponse<SolicitarCobroResponseDto>>
{
    /// <summary>UserId extraido del claim 'sub' del JWT.</summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Importe a retirar. Debe ser mayor que 0 y menor o igual al SaldoDisponible.
    /// Validacion de negocio (vs SaldoDisponible) se realiza en el Service.
    /// </summary>
    public decimal Importe { get; set; }

    /// <summary>Descripcion opcional del retiro. Max 500 caracteres.</summary>
    public string? Descripcion { get; set; }
}
```

**Implementa:** `IRequest<ServiceResponse<SolicitarCobroResponseDto>>`

**Propiedades del Command:**

| Propiedad | Tipo | Requerido | Origen | Validacion |
|-----------|------|-----------|--------|------------|
| `UserId` | `string` | Si | JWT claim `sub` | NotEmpty |
| `Importe` | `decimal` | Si | Request body | NotEmpty (1001), GreaterThan(0m) (1021) |
| `Descripcion` | `string?` | No | Request body | MaximumLength(500) (1002) si no es null |

**Handler - dependencias inyectadas:**

| Dependencia | Tipo | Proposito |
|-------------|------|-----------|
| `IPromotorWalletService` | `IPromotorWalletService` | Resolver promotor, wallet y ejecutar el cobro |
| `IValidator<SolicitarCobroCommand>` | `IValidator<T>` | Validar campos del command |
| `ILogger<SolicitarCobroCommandHandler>` | `ILogger<T>` | Logging de errores |

**Flujo del Handler:**

```
1. Ejecutar validator -> si invalido retornar ServiceResponse con mensajes de error
2. Resolver PromotorId desde UserId via IPromotorWalletService.GetPromotorByUserIdAsync
   └─> null => retornar ServiceResponse con NotFound_Promotor (2015)
3. Ejecutar IPromotorWalletService.SolicitarCobroAsync(promotorId, Importe, Descripcion, ct)
   (el service maneja internamente: GetWallet -> validaciones de negocio -> crear transaccion
    -> actualizar saldo con concurrencia optimista -> retornar SolicitarCobroResponseDto)
4. Retornar ServiceResponse<SolicitarCobroResponseDto>:
   - IsSuccess => Data con el DTO + Message con ErrorCode "0001" (Created)
   - IsFailure => Data null + Messages con el error del service
```

**Nota sobre validaciones de negocio en el Service:**
Las validaciones de negocio (saldo insuficiente 4040, saldo bajo minimo 4041, cobro concurrente 4042)
se realizan en `IPromotorWalletService.SolicitarCobroAsync`. El Service retorna una entidad o lanza
una excepcion de dominio. El Handler captura y convierte a ServiceResponse apropiado.
Alternativa: el service puede retornar un `Result<PromotorWalletTransaccion, string>` con el ErrorCode.
La decision de patron exacto queda para el implementador, siempre que el Handler NO inyecte DbContext.

---

## 5. Response DTOs

### 5.1 PromotorWalletDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorWalletDto.cs`

| Propiedad | Tipo C# | Nullable | Descripcion |
|-----------|---------|----------|-------------|
| `WalletId` | `Guid` | No | ID del wallet (PK de PromotorWallet) |
| `MonedaId` | `int` | No | FK a MaestraMoneda |
| `MonedaNombre` | `string` | No | Nombre de la moneda (ej: "EUR") |
| `SaldoDisponible` | `decimal` | No | Saldo disponible para retiro |
| `SaldoPendiente` | `decimal` | No | Saldo en transacciones pendientes |
| `TotalGanado` | `decimal` | No | Acumulado historico de creditos |
| `TotalRetirado` | `decimal` | No | Acumulado historico de debitos/retiros |
| `MinimoRetiro` | `decimal` | No | Importe minimo para solicitar cobro (constante de dominio = 10.00) |

**Estructura C# exacta:**

```csharp
namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromotorWalletDto
{
    /// <summary>ID del wallet.</summary>
    public Guid WalletId { get; set; }

    /// <summary>ID de la moneda (FK a MaestraMoneda).</summary>
    public int MonedaId { get; set; }

    /// <summary>Nombre de la moneda (ej: "EUR").</summary>
    public string MonedaNombre { get; set; } = null!;

    /// <summary>Saldo disponible para retiro.</summary>
    public decimal SaldoDisponible { get; set; }

    /// <summary>Saldo en transacciones pendientes.</summary>
    public decimal SaldoPendiente { get; set; }

    /// <summary>Acumulado historico de creditos.</summary>
    public decimal TotalGanado { get; set; }

    /// <summary>Acumulado historico de debitos/retiros.</summary>
    public decimal TotalRetirado { get; set; }

    /// <summary>
    /// Importe minimo para solicitar un cobro.
    /// Valor de configuracion (Crowdpromotion:MinimoRetiro), default 10.00.
    /// Se inyecta en el Handler desde IConfiguration, no se mapea desde la entidad.
    /// </summary>
    public decimal MinimoRetiro { get; set; }
}
```

**Nota:** `MinimoRetiro` NO existe en la entidad `PromotorWallet`. El Handler lo lee de
`IConfiguration["Crowdpromotion:MinimoRetiro"]` y lo asigna manualmente al DTO tras el mapeo
de AutoMapper. El mapping de AutoMapper solo cubre los campos de la entidad.

---

### 5.2 WalletTransaccionItemDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/WalletTransaccionItemDto.cs`

| Propiedad | Tipo C# | Nullable | Descripcion |
|-----------|---------|----------|-------------|
| `Id` | `Guid` | No | PK de PromotorWalletTransaccion |
| `EsCredito` | `bool` | No | `true` = ingreso/comision, `false` = retiro |
| `Importe` | `decimal` | No | Importe absoluto (siempre positivo) |
| `Descripcion` | `string?` | Si | Texto legible del origen, max 500 chars |
| `Concepto` | `string?` | Si | Etiqueta interna corta |
| `EstadoTransaccionId` | `int` | No | FK a MaestraEstadoWalletTransaccion |
| `EstadoTransaccionNombre` | `string` | No | Nombre del estado (ej: "Procesada") |
| `TipoRewardId` | `int?` | Si | FK a MaestraTipoRewardPromo. Null para retiros |
| `TipoRewardNombre` | `string?` | Si | Nombre del tipo de reward. Null para retiros |
| `PromoEventoId` | `Guid?` | Si | FK a PromoEvento que origino el credito. Null para retiros |
| `FechaCreacion` | `DateTime` | No | Fecha de creacion de la transaccion |
| `FechaProcesado` | `DateTime?` | Si | Fecha de procesamiento. Null si aun no procesada |

**Estructura C# exacta:**

```csharp
namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class WalletTransaccionItemDto
{
    /// <summary>Identificador unico de la transaccion.</summary>
    public Guid Id { get; set; }

    /// <summary>true = credito/ingreso (comision acreditada); false = debito/retiro.</summary>
    public bool EsCredito { get; set; }

    /// <summary>Importe absoluto de la transaccion. Siempre positivo.</summary>
    public decimal Importe { get; set; }

    /// <summary>Texto legible del origen de la transaccion. Max 500 chars. Nullable.</summary>
    public string? Descripcion { get; set; }

    /// <summary>Etiqueta interna corta. Nullable.</summary>
    public string? Concepto { get; set; }

    /// <summary>ID del estado de la transaccion (FK a MaestraEstadoWalletTransaccion).</summary>
    public int EstadoTransaccionId { get; set; }

    /// <summary>Nombre del estado (ej: "Pendiente", "Procesada", "Pagada", "Cancelada").</summary>
    public string EstadoTransaccionNombre { get; set; } = null!;

    /// <summary>ID del tipo de reward que origino el credito. Null para retiros.</summary>
    public int? TipoRewardId { get; set; }

    /// <summary>Nombre del tipo de reward. Null si no aplica (retiros).</summary>
    public string? TipoRewardNombre { get; set; }

    /// <summary>
    /// ID del PromoEvento que origino este credito.
    /// Null para transacciones de cobro/debito.
    /// </summary>
    public Guid? PromoEventoId { get; set; }

    /// <summary>Fecha de creacion del registro de transaccion.</summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>Fecha en que la transaccion fue procesada. Null si aun no fue procesada.</summary>
    public DateTime? FechaProcesado { get; set; }
}
```

---

### 5.3 WalletTransaccionesPagedDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/WalletTransaccionesPagedDto.cs`

| Propiedad | Tipo C# | Descripcion |
|-----------|---------|-------------|
| `Items` | `List<WalletTransaccionItemDto>` | Items de la pagina actual |
| `TotalCount` | `int` | Total de registros que cumplen los filtros |
| `Page` | `int` | Pagina actual (1-based) |
| `PageSize` | `int` | Tamano de pagina solicitado |
| `TotalPages` | `int` | Total de paginas calculado: `(int)Math.Ceiling((double)TotalCount / PageSize)` |

**Estructura C# exacta:**

```csharp
namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class WalletTransaccionesPagedDto
{
    /// <summary>Items de la pagina actual.</summary>
    public List<WalletTransaccionItemDto> Items { get; set; } = new();

    /// <summary>Total de registros que cumplen los filtros activos.</summary>
    public int TotalCount { get; set; }

    /// <summary>Numero de pagina actual (1-based).</summary>
    public int Page { get; set; }

    /// <summary>Tamano de pagina solicitado.</summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de paginas disponibles.
    /// Calculado como (int)Math.Ceiling((double)TotalCount / PageSize).
    /// </summary>
    public int TotalPages { get; set; }
}
```

---

### 5.4 SolicitarCobroResponseDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/SolicitarCobroResponseDto.cs`

| Propiedad | Tipo C# | Nullable | Descripcion |
|-----------|---------|----------|-------------|
| `TransaccionId` | `Guid` | No | ID de la PromotorWalletTransaccion creada |
| `Importe` | `decimal` | No | Importe del retiro solicitado |
| `MonedaNombre` | `string` | No | Nombre de la moneda del wallet (ej: "EUR") |
| `EstadoTransaccionNombre` | `string` | No | Siempre "Pendiente" en MVP |
| `SaldoRestante` | `decimal` | No | SaldoDisponible del wallet tras descontar el retiro |
| `FechaCreacion` | `DateTime` | No | Fecha de creacion de la transaccion |

**Estructura C# exacta:**

```csharp
namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class SolicitarCobroResponseDto
{
    /// <summary>ID de la PromotorWalletTransaccion de debito creada.</summary>
    public Guid TransaccionId { get; set; }

    /// <summary>Importe del retiro solicitado.</summary>
    public decimal Importe { get; set; }

    /// <summary>Nombre de la moneda del wallet (ej: "EUR").</summary>
    public string MonedaNombre { get; set; } = null!;

    /// <summary>
    /// Nombre del estado de la transaccion creada.
    /// Siempre "Pendiente" (EstadoTransaccionId = 1) en MVP.
    /// </summary>
    public string EstadoTransaccionNombre { get; set; } = null!;

    /// <summary>
    /// SaldoDisponible del wallet tras descontar el retiro solicitado.
    /// = SaldoDisponible_anterior - Importe.
    /// </summary>
    public decimal SaldoRestante { get; set; }

    /// <summary>Fecha UTC de creacion de la transaccion.</summary>
    public DateTime FechaCreacion { get; set; }
}
```

---

## 6. Validadores FluentValidation

### 6.1 GetPromotorWalletQueryValidator

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Validators/GetPromotorWalletQueryValidator.cs`

Esta query no tiene parametros de entrada del cliente (todo se extrae del JWT), por lo que el
validator solo verifica la presencia del UserId que el Controller debe siempre proporcionar.

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `UserId` | `NotEmpty()` | "El identificador de usuario es obligatorio" | `Validation_Required` (1001) |

**Estructura C# exacta:**

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

### 6.2 GetWalletTransaccionesQueryValidator

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Validators/GetWalletTransaccionesQueryValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `UserId` | `NotEmpty()` | "El identificador de usuario es obligatorio" | `Validation_Required` (1001) |
| `Page` | `GreaterThanOrEqualTo(1)` | "La pagina debe ser mayor o igual a 1" | `Validation_RangeOutOfBounds` (1021) |
| `PageSize` | `InclusiveBetween(1, 50)` | "El tamano de pagina debe estar entre 1 y 50" | `Validation_RangeOutOfBounds` (1021) |
| `EstadoTransaccionId` | `InclusiveBetween(1, 4)` cuando no es null | "El estado de transaccion debe ser entre 1 y 4" | `Validation_RangeOutOfBounds` (1021) |
| `FechaDesde + FechaHasta` | Custom: `FechaDesde <= FechaHasta` cuando ambas presentes | "La fecha de inicio no puede ser posterior a la fecha de fin" | `Validation_FechaRangoInvalido` (1036) |

**Estructura C# exacta:**

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

### 6.3 SolicitarCobroCommandValidator

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Validators/SolicitarCobroCommandValidator.cs`

**Nota critica:** El validator solo valida campos de formato/estructura. Las validaciones de negocio
(`importe > SaldoDisponible`, `SaldoDisponible < MinimoRetiro`, `CobroConcurrente`) se realizan
en el Service (REGLA 3: Handler no inyecta DbContext). El validator no inyecta ningun servicio.

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `UserId` | `NotEmpty()` | "El identificador de usuario es obligatorio" | `Validation_Required` (1001) |
| `Importe` | `GreaterThan(0m)` | "El importe debe ser mayor que cero" | `Validation_RangeOutOfBounds` (1021) |
| `Descripcion` | `MaximumLength(500)` cuando no es null | "La descripcion no puede superar los 500 caracteres" | `Validation_MaxLength` (1002) |

**Estructura C# exacta:**

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

## 7. Nueva Interfaz de Servicio

### 7.1 IPromotorWalletService

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromotorWalletService.cs`

Este es un servicio nuevo. Los handlers de esta feature dependen exclusivamente de el.

| Metodo | Firma | Descripcion |
|--------|-------|-------------|
| `GetPromotorByUserIdAsync` | `Task<Promotor?> GetPromotorByUserIdAsync(string userId, CancellationToken ct)` | Reutiliza logica existente de `IPromoEventoService`. Resuelve el perfil de promotor desde el UserId |
| `GetWalletByPromotorIdAsync` | `Task<PromotorWallet?> GetWalletByPromotorIdAsync(PromotorId promotorId, CancellationToken ct)` | Retorna null si el promotor no tiene wallet |
| `GetTransaccionesPagedAsync` | `Task<(IReadOnlyList<PromotorWalletTransaccion> Items, int TotalCount)> GetTransaccionesPagedAsync(Guid walletId, bool? esCredito, int? estadoTransaccionId, DateTime? fechaDesde, DateTime? fechaHasta, int page, int pageSize, CancellationToken ct)` | Retorna lista paginada y total count para el paginado |
| `SolicitarCobroAsync` | `Task<SolicitarCobroResult> SolicitarCobroAsync(PromotorId promotorId, decimal importe, string? descripcion, CancellationToken ct)` | Contiene toda la logica de negocio del cobro (validaciones + persistencia atomica) |

**SolicitarCobroResult:** Tipo de retorno del servicio que encapsula tanto el exito como los
distintos errores de negocio. Puede implementarse como un `record` con discriminated union o
como excepcion de dominio. El implementador elige el patron, pero el Handler debe poder distinguir
entre exito y cada uno de los tres errores de negocio (4040, 4041, 4042).

```csharp
/// <summary>
/// Resultado de la operacion SolicitarCobro del service.
/// Encapsula exito, datos de la transaccion creada y posibles errores de negocio.
/// </summary>
public record SolicitarCobroResult(
    bool IsSuccess,
    PromotorWalletTransaccion? Transaccion,
    PromotorWallet? WalletActualizada,
    string? MonedaNombre,
    string? ErrorCode,
    string? ErrorMessage
);
```

**Nota de reutilizacion:** El metodo `GetPromotorByUserIdAsync` puede reutilizar la implementacion
existente de `IPromoEventoService`. Se puede inyectar `IPromoEventoService` dentro de
`PromotorWalletService`, o bien crear un metodo compartido en un servicio base. La decision queda
para el implementador segun el patron que mejor encaje con la arquitectura existente.

---

## 8. AutoMapper Mappings

### 8.1 PromotorWalletProfile

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Mapping/PromotorWalletProfile.cs`

Un profile por entidad (no agregar al `PromoEventoProfile.cs` existente).

| Source | Destination | Notas de Mapping |
|--------|-------------|-----------------|
| `PromotorWallet` | `PromotorWalletDto` | Mapeo directo de todos los campos. `MinimoRetiro` se ignora en AutoMapper y se asigna manualmente en el Handler. `WalletId` mapea desde `Id` de la entidad |
| `PromotorWalletTransaccion` | `WalletTransaccionItemDto` | Ver detalle de campos calculados abajo |

**Mappings detallados:**

**PromotorWallet -> PromotorWalletDto:**

| Campo entidad | Campo DTO | Tipo mapping |
|---------------|-----------|--------------|
| `Id` | `WalletId` | `ForMember` con `MapFrom(src => src.Id)` |
| `MonedaId` | `MonedaId` | Automatico |
| `Moneda.Nombre` (navegacion) | `MonedaNombre` | `ForMember` con `MapFrom(src => src.Moneda.Nombre)` |
| `SaldoDisponible` | `SaldoDisponible` | Automatico |
| `SaldoPendiente` | `SaldoPendiente` | Automatico |
| `TotalGanado` | `TotalGanado` | Automatico |
| `TotalRetirado` | `TotalRetirado` | Automatico |
| `(no existe)` | `MinimoRetiro` | `Ignore()` - se asigna en el Handler desde IConfiguration |

**PromotorWalletTransaccion -> WalletTransaccionItemDto:**

| Campo entidad | Campo DTO | Tipo mapping |
|---------------|-----------|--------------|
| `Id` | `Id` | Automatico |
| `EsCredito` | `EsCredito` | Automatico (campo nuevo de dominio) |
| `Importe` | `Importe` | Automatico |
| `Descripcion` | `Descripcion` | Automatico (campo nuevo de dominio) |
| `Concepto` | `Concepto` | Automatico |
| `EstadoTransaccionId` | `EstadoTransaccionId` | Automatico |
| `EstadoTransaccion.Nombre` (nav.) | `EstadoTransaccionNombre` | `ForMember` con `MapFrom(src => src.EstadoTransaccion.Nombre)` |
| `TipoRewardId` | `TipoRewardId` | Automatico |
| `TipoReward.Nombre` (nav.) | `TipoRewardNombre` | `ForMember` con `MapFrom(src => src.TipoReward != null ? src.TipoReward.Nombre : null)` |
| `PromoEventoId` | `PromoEventoId` | Automatico (campo nuevo de dominio) |
| `FechaCreacion` | `FechaCreacion` | Automatico |
| `FechaProcesado` | `FechaProcesado` | Automatico |

**Estructura C# del Profile:**

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
        CreateMap<PromotorWallet, PromotorWalletDto>()
            .ForMember(dest => dest.WalletId,
                       opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MonedaNombre,
                       opt => opt.MapFrom(src => src.Moneda.Nombre))
            .ForMember(dest => dest.MinimoRetiro,
                       opt => opt.Ignore()); // Se asigna desde IConfiguration en el Handler

        // PromotorWalletTransaccion -> WalletTransaccionItemDto
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

**Nota sobre includes EF Core:** Para que los mappings de navegacion funcionen, el Repository debe
hacer `Include(x => x.Moneda)` al cargar `PromotorWallet`, e `Include(x => x.EstadoTransaccion)`
y `Include(x => x.TipoReward)` al cargar `PromotorWalletTransaccion`.

---

## 9. Controller Methods (PromotorController)

**Archivo existente a modificar:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/Controllers/PromotorController.cs`

Se agregan tres metodos al `PromotorController` existente. El controller ya tiene `IMediator` y
`ICurrentUserService` inyectados como dependencias (ver implementacion existente).

### 9.1 GetWallet

```
[HttpGet("wallet")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromotorWalletDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<PromotorWalletDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<PromotorWalletDto>), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ServiceResponse<PromotorWalletDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<PromotorWalletDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetWallet(CancellationToken cancellationToken)
```

**Logica del metodo:**

```
1. Verificar _currentUser.UserId != null
   └─> null => return Unauthorized con Auth_InvalidToken (3004)
2. Construir GetPromotorWalletQuery { UserId = userId.Value.ToString() }
3. var result = await _mediator.Send(query, cancellationToken)
4. return FromServiceResponse(result)
```

**Nota:** `FromServiceResponse` es el metodo heredado de `BaseLoggerController` que convierte
el `ServiceResponse` en el `IActionResult` con el HTTP status code correcto segun el ErrorCode.

---

### 9.2 GetWalletTransacciones

```
[HttpGet("wallet/transacciones")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<WalletTransaccionesPagedDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<WalletTransaccionesPagedDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<WalletTransaccionesPagedDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<WalletTransaccionesPagedDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<WalletTransaccionesPagedDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetWalletTransacciones(
    [FromQuery] bool? esCredito,
    [FromQuery] int? estadoTransaccionId,
    [FromQuery] string? fechaDesde,
    [FromQuery] string? fechaHasta,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
```

**Logica del metodo:**

```
1. Verificar _currentUser.UserId != null
   └─> null => return Unauthorized con Auth_InvalidToken (3004)
2. Parsear fechaDesde y fechaHasta de string ISO 8601 a DateTime? con DateTime.TryParse
   └─> formato invalido => return BadRequest con Validation_FechaRangoInvalido (1036)
   Patron igual al metodo GetMetricas existente (pero con DateTime en lugar de DateOnly)
3. Construir GetWalletTransaccionesQuery con todos los parametros
4. var result = await _mediator.Send(query, cancellationToken)
5. return FromServiceResponse(result)
```

**Nota sobre fechas:** El endpoint acepta fechas en formato ISO 8601 completo (DateTime) o solo fecha
(yyyy-MM-dd). El Controller realiza el parseo y pasa `DateTime?` al Query, igual que el patron
del `GetMetricas` existente en el mismo controller.

---

### 9.3 SolicitarCobro

```
[HttpPost("wallet/cobro")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<SolicitarCobroResponseDto>), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ServiceResponse<SolicitarCobroResponseDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<SolicitarCobroResponseDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<SolicitarCobroResponseDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<SolicitarCobroResponseDto>), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ServiceResponse<SolicitarCobroResponseDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> SolicitarCobro(
    [FromBody] SolicitarCobroRequestDto request,
    CancellationToken cancellationToken)
```

**Logica del metodo:**

```
1. Verificar _currentUser.UserId != null
   └─> null => return Unauthorized con Auth_InvalidToken (3004)
2. Construir SolicitarCobroCommand:
   {
       UserId = userId.Value.ToString(),
       Importe = request.Importe,
       Descripcion = request.Descripcion
   }
3. var result = await _mediator.Send(command, cancellationToken)
4. if result.IsSuccess => return CreatedAtAction(nameof(GetWallet), null, result)
5. return FromServiceResponse(result)
```

**SolicitarCobroRequestDto:** DTO de request del body (solo para el Controller, no es un Command).

```csharp
// Archivo: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/SolicitarCobroRequestDto.cs
namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class SolicitarCobroRequestDto
{
    /// <summary>Importe a retirar. Debe ser mayor que 0.</summary>
    public decimal Importe { get; set; }

    /// <summary>Descripcion opcional del retiro. Max 500 caracteres.</summary>
    public string? Descripcion { get; set; }
}
```

**Nota de diseno:** El `SolicitarCobroRequestDto` es el objeto que llega en el body HTTP.
El Controller lo convierte en `SolicitarCobroCommand` agregando el `UserId` del JWT.
Esto sigue el mismo patron del `PromotorController.Create` existente donde `CreatePromotorRequestDto`
se convierte en `CreatePromotorCommand`.

---

## 10. Tabla de Errores HTTP Completa

### GET /api/crowdpromotion/promotor/wallet

| HTTP | ErrorCode | Constante | Mensaje | Causa |
|------|-----------|-----------|---------|-------|
| 200 | - | - | - | Exito: wallet retornado |
| 401 | 3004 | `Auth_InvalidToken` | "Token no valido o expirado" | Token ausente o invalido (verificacion en Controller) |
| 401 | 3001 | `Auth_Unauthorized` | "No autorizado" | No autenticado |
| 403 | 3002 | `Auth_Forbidden` | "Forbidden" | Token valido pero sin permiso |
| 404 | 2015 | `NotFound_Promotor` | "No tienes un perfil de promotor" | UserId sin perfil de promotor |
| 404 | 2030 | `NotFound_Wallet` | "Wallet no encontrado" | Promotor sin wallet asignado |
| 500 | 5000 | `Internal_UnexpectedError` | "Error inesperado" | Error interno del servidor |

### GET /api/crowdpromotion/promotor/wallet/transacciones

| HTTP | ErrorCode | Constante | Mensaje | Causa |
|------|-----------|-----------|---------|-------|
| 200 | - | - | - | Exito: lista paginada retornada |
| 400 | 1021 | `Validation_RangeOutOfBounds` | "La pagina debe ser mayor o igual a 1" | `page < 1` o `pageSize` fuera de [1,50] |
| 400 | 1036 | `Validation_FechaRangoInvalido` | "La fecha de inicio no puede ser posterior a la fecha de fin" | `fechaDesde > fechaHasta` |
| 401 | 3004 | `Auth_InvalidToken` | "Token no valido o expirado" | Token ausente |
| 404 | 2015 | `NotFound_Promotor` | "No tienes un perfil de promotor" | UserId sin perfil de promotor |
| 404 | 2030 | `NotFound_Wallet` | "Wallet no encontrado" | Promotor sin wallet |
| 500 | 5000 | `Internal_UnexpectedError` | "Error inesperado" | Error interno |

### POST /api/crowdpromotion/promotor/wallet/cobro

| HTTP | ErrorCode | Constante | Mensaje | Causa |
|------|-----------|-----------|---------|-------|
| 201 | 0001 | `Created` | "Solicitud de cobro registrada" | Exito: transaccion de debito creada |
| 400 | 1001 | `Validation_Required` | "El importe es obligatorio" | `importe` no enviado en el body |
| 400 | 1021 | `Validation_RangeOutOfBounds` | "El importe debe ser mayor que cero" | `importe <= 0` |
| 400 | 1002 | `Validation_MaxLength` | "La descripcion no puede superar los 500 caracteres" | `descripcion.length > 500` |
| 401 | 3004 | `Auth_InvalidToken` | "Token no valido o expirado" | Token ausente |
| 404 | 2015 | `NotFound_Promotor` | "No tienes un perfil de promotor" | UserId sin perfil de promotor |
| 404 | 2030 | `NotFound_Wallet` | "Wallet no encontrado" | Promotor sin wallet |
| 409 | 4040 | `BusinessRule_SaldoInsuficiente` | "Saldo insuficiente" | `importe > SaldoDisponible` |
| 409 | 4041 | `BusinessRule_SaldoBajoMinimoRetiro` | "Saldo bajo el minimo de retiro" | `SaldoDisponible < MinimoRetiro (10.00)` |
| 409 | 4042 | `BusinessRule_CobroConcurrente` | "Ya existe un cobro pendiente en proceso" | Conflicto de RowVersion o transaccion pendiente activa |
| 500 | 5000 | `Internal_UnexpectedError` | "Error inesperado" | Error interno |

**Nota sobre HTTP 409 vs 400:** Los errores de negocio (4040, 4041, 4042) se retornan como 409 Conflict
ya que son conflictos de estado del recurso (saldo insuficiente, cobro concurrente), no errores de
formato de los datos de entrada. El metodo `FromServiceResponse` de `BaseLoggerController` debe
mapear ErrorCodes que empiezan por "40" a HTTP 409. Si la implementacion actual no soporta este
mapeo, el implementador debe agregar la logica en el Controller directamente.

---

## 11. OpenAPI / Swagger Documentation

### GET /api/crowdpromotion/promotor/wallet

```
Summary: Obtener resumen del wallet del promotor autenticado
Description: Retorna el saldo disponible, totales historicos, moneda y el importe minimo de retiro
             del wallet del promotor. El PromotorId se extrae del JWT, no se requiere parametro.
             Retorna 404 si el usuario no tiene perfil de promotor o si aun no tiene wallet.
Tags: [Crowdpromotion, Wallet]
Security: [BearerAuth]
Parameters: ninguno
Responses:
  200: ServiceResponse<PromotorWalletDto>
  401: ServiceResponse (token ausente/invalido)
  404: ServiceResponse (2015 = sin perfil promotor, 2030 = sin wallet)
  500: ServiceResponse (5000 = error interno)
```

### GET /api/crowdpromotion/promotor/wallet/transacciones

```
Summary: Obtener historial paginado de transacciones del wallet
Description: Retorna la lista paginada de transacciones del wallet del promotor autenticado.
             Soporta filtros por tipo (credito/debito), estado y rango de fechas.
             Las transacciones se ordenan por FechaCreacion DESC (mas reciente primero).
Tags: [Crowdpromotion, Wallet]
Security: [BearerAuth]
Parameters:
  - esCredito (query, bool?, opcional): true = solo ingresos; false = solo retiros; omitir = todos
  - estadoTransaccionId (query, int?, opcional): 1=Pendiente, 2=Procesada, 3=Pagada, 4=Cancelada
  - fechaDesde (query, string, opcional): ISO 8601. Filtro desde fecha inclusive
  - fechaHasta (query, string, opcional): ISO 8601. Filtro hasta fecha inclusive
  - page (query, int, opcional, default=1): Numero de pagina (minimo 1)
  - pageSize (query, int, opcional, default=10): Items por pagina (minimo 1, maximo 50)
Responses:
  200: ServiceResponse<WalletTransaccionesPagedDto>
  400: ServiceResponse (1021 = pagina/pageSize invalido, 1036 = rango fechas invalido)
  401: ServiceResponse (token ausente/invalido)
  404: ServiceResponse (2015 = sin perfil promotor, 2030 = sin wallet)
  500: ServiceResponse (5000 = error interno)
```

### POST /api/crowdpromotion/promotor/wallet/cobro

```
Summary: Solicitar cobro/retiro desde el wallet
Description: Registra una solicitud de cobro del saldo disponible del promotor.
             Crea una transaccion de debito en estado Pendiente y descuenta el importe
             del saldo disponible de forma atomica. El procesamiento real del pago es manual en MVP.
             Requiere que el saldo disponible sea mayor o igual al minimo de retiro (10.00).
Tags: [Crowdpromotion, Wallet]
Security: [BearerAuth]
RequestBody:
  Content-Type: application/json
  Schema: SolicitarCobroRequestDto
    - importe (decimal, requerido): Importe a retirar. Debe ser > 0 y <= SaldoDisponible
    - descripcion (string, opcional): Descripcion del retiro. Max 500 caracteres
Responses:
  201: ServiceResponse<SolicitarCobroResponseDto> con ErrorCode "0001"
  400: ServiceResponse (1001 = importe requerido, 1021 = importe <= 0, 1002 = descripcion > 500 chars)
  401: ServiceResponse (token ausente/invalido)
  404: ServiceResponse (2015 = sin perfil promotor, 2030 = sin wallet)
  409: ServiceResponse (4040 = saldo insuficiente, 4041 = saldo bajo minimo, 4042 = cobro concurrente)
  500: ServiceResponse (5000 = error interno)
```

---

## 12. Configuracion de Appsettings

El minimo de retiro se configura en `appsettings.json` (no hardcodeado, RNF-02).

**Clave:** `Crowdpromotion:MinimoRetiro`
**Tipo:** `decimal`
**Default:** `10.0`

```json
// appsettings.json (agregar dentro de la seccion existente o crear nueva)
{
  "Crowdpromotion": {
    "MinimoRetiro": 10.0
  }
}
```

El Handler de `GetPromotorWalletQuery` y el Service de `SolicitarCobroAsync` deben leer este valor
via `IConfiguration`. La inyeccion de `IConfiguration` se hace en el constructor del Handler o
del Service segun donde se use.

---

## 13. Archivos a Crear

```
Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/
├── Dtos/
│   ├── PromotorWalletDto.cs                         NUEVO
│   ├── WalletTransaccionItemDto.cs                  NUEVO
│   ├── WalletTransaccionesPagedDto.cs               NUEVO
│   ├── SolicitarCobroRequestDto.cs                  NUEVO
│   └── SolicitarCobroResponseDto.cs                 NUEVO
├── Features/Wallet/
│   ├── Commands/
│   │   └── SolicitarCobroCommand.cs                 NUEVO (Command + Handler)
│   ├── Queries/
│   │   ├── GetPromotorWalletQuery.cs                NUEVO (Query + Handler)
│   │   └── GetWalletTransaccionesQuery.cs           NUEVO (Query + Handler)
│   └── Validators/
│       ├── GetPromotorWalletQueryValidator.cs       NUEVO
│       ├── GetWalletTransaccionesQueryValidator.cs  NUEVO
│       └── SolicitarCobroCommandValidator.cs        NUEVO
├── Interfaces/Services/
│   └── IPromotorWalletService.cs                   NUEVO
└── Mapping/
    └── PromotorWalletProfile.cs                    NUEVO

Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/
└── Constants/
    └── ServiceResponseMessageType.cs               MODIFICAR (agregar 4 constantes nuevas)

Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/
└── Controllers/
    └── PromotorController.cs                       MODIFICAR (agregar 3 metodos)
```

**Archivos fuera del scope de este plan (responsabilidad de otros planes):**

```
Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/
├── Services/
│   └── PromotorWalletService.cs                    (plan de arquitectura hexagonal)
└── Repositories/
    └── PromotorWalletRepository.cs                 (plan de arquitectura hexagonal)
```

---

## 14. Checklist de Contratos

### Commands y Queries
- [ ] `GetPromotorWalletQuery` implementa `IRequest<ServiceResponse<PromotorWalletDto>>`
- [ ] `GetWalletTransaccionesQuery` implementa `IRequest<ServiceResponse<WalletTransaccionesPagedDto>>`
- [ ] `SolicitarCobroCommand` implementa `IRequest<ServiceResponse<SolicitarCobroResponseDto>>`
- [ ] Handler + Command/Query en MISMO archivo (REGLA 2 CQRS)

### Handlers
- [ ] Handlers inyectan `IPromotorWalletService`, NO `DbContext` (REGLA 3 CQRS)
- [ ] Handlers inyectan `ILogger<T>` (REGLA 9 CQRS)
- [ ] Handlers inyectan `IValidator<T>` donde aplica
- [ ] Constructores con `?? throw new ArgumentNullException` para todas las dependencias (REGLA 6 CQRS)
- [ ] Retornan `ServiceResponse<T>` con validacion retornando response (no throw) (REGLA 7 CQRS)
- [ ] Try-Catch con logging en todos los handlers (REGLA 9 CQRS)

### Validators
- [ ] `GetPromotorWalletQueryValidator` - UserId NotEmpty con WithMessage + WithErrorCode
- [ ] `GetWalletTransaccionesQueryValidator` - Page, PageSize, EstadoId, FechaRango con WithMessage + WithErrorCode
- [ ] `SolicitarCobroCommandValidator` - Importe GreaterThan, Descripcion MaxLength con WithMessage + WithErrorCode
- [ ] Todos los validators usan `ServiceResponseMessageType.X` (no strings literales) (REGLA 5 CQRS)
- [ ] Todas las reglas tienen `.WithMessage()` Y `.WithErrorCode()` (REGLA 5 CQRS)

### DTOs
- [ ] `PromotorWalletDto` - 8 propiedades incluyendo `MinimoRetiro`
- [ ] `WalletTransaccionItemDto` - 12 propiedades incluyendo `EsCredito`, `PromoEventoId`, `Descripcion`
- [ ] `WalletTransaccionesPagedDto` - Items + paginacion (TotalCount, Page, PageSize, TotalPages)
- [ ] `SolicitarCobroRequestDto` - Importe + Descripcion (DTO del body HTTP)
- [ ] `SolicitarCobroResponseDto` - 6 propiedades incluyendo SaldoRestante

### AutoMapper
- [ ] `PromotorWalletProfile` creado como profile independiente (no agregar a PromoEventoProfile)
- [ ] `PromotorWallet` -> `PromotorWalletDto` con `WalletId` mapeado desde `Id` e `Ignore()` en `MinimoRetiro`
- [ ] `PromotorWalletTransaccion` -> `WalletTransaccionItemDto` con navegaciones de estado y tipo
- [ ] Profile registrado en DependencyInjection del modulo Application

### Constantes
- [ ] `NotFound_Wallet = "2030"` agregado en `ServiceResponseMessageType`
- [ ] `BusinessRule_SaldoInsuficiente = "4040"` agregado
- [ ] `BusinessRule_SaldoBajoMinimoRetiro = "4041"` agregado
- [ ] `BusinessRule_CobroConcurrente = "4042"` agregado

### Controller
- [ ] `GetWallet` con `[HttpGet("wallet")]` y `[Authorize]`
- [ ] `GetWalletTransacciones` con `[HttpGet("wallet/transacciones")]` y `[Authorize]`
- [ ] `SolicitarCobro` con `[HttpPost("wallet/cobro")]` y `[Authorize]`
- [ ] Todos los metodos con `[ProducesResponseType]` para todos los HTTP codes posibles
- [ ] `SolicitarCobro` retorna 201 Created en exito con `CreatedAtAction`
- [ ] Parseo de fechas ISO 8601 en Controller (igual que `GetMetricas` existente)

### Modelo de Dominio
- [ ] `PromotorWalletTransaccion.EsCredito` (bool, NOT NULL) existe o se planifica migracion
- [ ] `PromotorWalletTransaccion.PromoEventoId` (Guid?, NULL) existe o se planifica migracion
- [ ] `PromotorWalletTransaccion.Descripcion` (string?, NULL, max 500) existe o se planifica migracion
- [ ] Migracion `AddWalletTransaccionFields` con default `EsCredito = true` para filas existentes

### Configuracion
- [ ] `appsettings.json` tiene clave `Crowdpromotion:MinimoRetiro` con valor `10.0`
- [ ] Handler/Service lee `MinimoRetiro` de `IConfiguration`, no hardcodeado (RNF-02)
