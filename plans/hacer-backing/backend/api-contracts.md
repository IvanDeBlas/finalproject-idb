# Contratos API: Hacer Backing

**Fecha:** 2026-02-13
**Modulo:** Crowdfunding
**Feature:** hacer-backing

---

## 1. Endpoints

| Metodo | Ruta | Tipo | Descripcion | Auth |
|--------|------|------|-------------|------|
| GET | /api/campanias | Query | Listar campanias activas con paginacion | Publico |
| GET | /api/campanias/{id} | Query | Obtener detalle completo de campania con rewards y backings | Publico |
| POST | /api/campanias/{id}/backings | Command | Crear nuevo backing (aporte a campania) | Opcional |
| GET | /api/campanias/{id}/backings | Query | Listar backings publicos de campania (paginado) | Publico |
| GET | /api/campanias/{id}/stats | Query | Obtener estadisticas agregadas de campania | Publico |

---

## 2. Request DTOs

### 2.1 CreateBackingCommand

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Backings/Commands/CreateBackingCommand.cs`

**Estructura:**
```csharp
namespace WePlayRises.Crowdfunding.Application.Features.Backings.Commands;

/// <summary>
/// Command para crear un nuevo backing (aporte) a una campania.
/// Crea PedidoCrowdfunding + PedidoCrowdfundingLinea + AportacionCrowdfunding simulada.
/// Actualiza ImportePledgedActual de la campania atomicamente.
/// </summary>
public class CreateBackingCommand : IRequest<ServiceResponse<BackingDto>>
{
    /// <summary>
    /// ID de la campania a la que se hace el aporte.
    /// Viene de la URL del endpoint (no del body).
    /// </summary>
    public Guid CampaniaId { get; set; }

    /// <summary>
    /// ID del reward seleccionado (opcional, null para aporte sin recompensa).
    /// Si se proporciona, el monto debe ser >= reward.ImporteMinimo.
    /// </summary>
    public Guid? RewardId { get; set; }

    /// <summary>
    /// Monto del aporte en la moneda de la campania.
    /// Debe ser >= 1.00 EUR (o moneda correspondiente).
    /// Si hay reward seleccionado, debe ser >= reward.ImporteMinimo.
    /// </summary>
    public decimal Monto { get; set; }

    /// <summary>
    /// Mensaje opcional del backer para el artista (max 500 caracteres).
    /// Se almacena en PedidoCrowdfunding.ComentarioBacker.
    /// </summary>
    public string? Mensaje { get; set; }

    /// <summary>
    /// Si es true, el nombre del backer NO se muestra en la lista publica.
    /// Se mapea a !PedidoCrowdfunding.PermitirMostrarNombre.
    /// </summary>
    public bool EsAnonimo { get; set; }

    // ===== PROPIEDADES INYECTADAS DESDE CONTROLLER (NO VIENEN EN BODY) =====

    /// <summary>
    /// ID del usuario autenticado (claim "sub" del JWT).
    /// Null si el usuario no esta autenticado.
    /// Asignado desde HttpContext.User en el controller.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Nombre del usuario autenticado (claim "name" del JWT).
    /// Null si el usuario no esta autenticado.
    /// Usado para mostrar el nombre en la lista publica de backings.
    /// Asignado desde HttpContext.User en el controller.
    /// </summary>
    public string? UserName { get; set; }
}
```

**Implementa:** `IRequest<ServiceResponse<BackingDto>>`

**Mapeos:**
- `CreateBackingCommand.Monto` → `PedidoCrowdfunding.ImporteSubtotal`, `ImporteTotal`
- `CreateBackingCommand.Mensaje` → `PedidoCrowdfunding.ComentarioBacker`
- `CreateBackingCommand.EsAnonimo` → `!PedidoCrowdfunding.PermitirMostrarNombre`
- `CreateBackingCommand.UserId` → `PedidoCrowdfunding.UserId` (nullable)

**Notas:**
- `CampaniaId`, `UserId`, `UserName` NO vienen en el body. Se asignan en el controller.
- En MVP, NO hay propinas ni envio: `ImportePropina = 0`, `ImporteEnvio = 0`, `ImporteImpuestos = 0`.
- `EstadoPedidoId = 3` (COMPLETADO) porque no hay integracion de pago real en MVP.

---

### 2.2 GetCampaniaDetailQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaDetailQuery.cs`

**Estructura:**
```csharp
namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;

/// <summary>
/// Query para obtener detalle completo de una campania con rewards activos y backings recientes.
/// Usado en la pagina publica de detalle de campania.
/// </summary>
public class GetCampaniaDetailQuery : IRequest<ServiceResponse<CampaniaDetailDto>>
{
    public Guid Id { get; set; }
}
```

**Implementa:** `IRequest<ServiceResponse<CampaniaDetailDto>>`

**Notas:**
- Similar a `GetCampaniaByIdQuery` existente, pero retorna `CampaniaDetailDto` en lugar de `CampaniaDto`.
- Incluye datos relacionados: rewards activos, backings recientes, total backers, artista.

---

### 2.3 GetBackingsByCampaniaQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Backings/Queries/GetBackingsByCampaniaQuery.cs`

**Estructura:**
```csharp
namespace WePlayRises.Crowdfunding.Application.Features.Backings.Queries;

/// <summary>
/// Query para obtener lista paginada de backings publicos de una campania.
/// Solo muestra backings con EstadoPedidoId = 3 (COMPLETADO).
/// Respeta el flag PermitirMostrarNombre para mostrar nombre o "Anonimo".
/// </summary>
public class GetBackingsByCampaniaQuery : IRequest<ServiceResponse<PaginatedList<BackingPublicDto>>>
{
    public Guid CampaniaId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
```

**Implementa:** `IRequest<ServiceResponse<PaginatedList<BackingPublicDto>>>`

**Notas:**
- `PaginatedList<T>` es el tipo existente usado en `GetAllCampaniasQuery`.
- Solo retorna backings con `EstadoPedidoId = 3` (COMPLETADO).
- Ordenados por `FechaCreacion DESC` (mas recientes primero).

---

### 2.4 GetCampaniaStatsQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaStatsQuery.cs`

**Estructura:**
```csharp
namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;

/// <summary>
/// Query para obtener estadisticas agregadas de backings de una campania.
/// Calcula metricas en base a PedidoCrowdfunding con EstadoPedidoId = 3 (COMPLETADO).
/// </summary>
public class GetCampaniaStatsQuery : IRequest<ServiceResponse<CampaniaStatsDto>>
{
    public Guid CampaniaId { get; set; }
}
```

**Implementa:** `IRequest<ServiceResponse<CampaniaStatsDto>>`

---

## 3. Response DTOs

### 3.1 BackingDto

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/BackingDto.cs`

**Estructura:**
```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// DTO completo de un backing (aporte) retornado tras crear exitosamente.
/// Contiene informacion del pedido, campania, reward y usuario.
/// </summary>
public class BackingDto
{
    /// <summary>
    /// ID del pedido creado (PedidoCrowdfundingId).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID de la campania apoyada.
    /// </summary>
    public Guid CampaniaId { get; set; }

    /// <summary>
    /// Titulo de la campania apoyada.
    /// </summary>
    public string CampaniaTitulo { get; set; } = null!;

    /// <summary>
    /// ID del reward seleccionado (null si aporte sin recompensa).
    /// </summary>
    public Guid? RewardId { get; set; }

    /// <summary>
    /// Nombre del reward seleccionado (null si no hay reward).
    /// </summary>
    public string? RewardNombre { get; set; }

    /// <summary>
    /// Monto del aporte.
    /// </summary>
    public decimal Monto { get; set; }

    /// <summary>
    /// Simbolo de la moneda (EUR, USD, etc.).
    /// </summary>
    public string MonedaSimbolo { get; set; } = null!;

    /// <summary>
    /// Mensaje opcional del backer.
    /// </summary>
    public string? Mensaje { get; set; }

    /// <summary>
    /// Si es true, el aporte fue anonimo.
    /// </summary>
    public bool EsAnonimo { get; set; }

    /// <summary>
    /// Nombre del backer (o "Anonimo" si EsAnonimo = true).
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Fecha de creacion del backing.
    /// </summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Estado del pedido (ej: "Completado").
    /// En MVP siempre es "Completado" porque no hay pago real.
    /// </summary>
    public string EstadoPedido { get; set; } = null!;
}
```

**Mapeos desde:**
- `PedidoCrowdfunding` (entidad principal)
- `CampaniaCrowdfunding.Titulo` → `CampaniaTitulo`
- `CampaniaCrowdfundingReward.Nombre` → `RewardNombre`
- `MaestraEstadoPedido.Nombre` → `EstadoPedido` (via lookup)
- `MaestraMoneda.Simbolo` → `MonedaSimbolo` (via lookup)
- `!PedidoCrowdfunding.PermitirMostrarNombre` → `EsAnonimo`

---

### 3.2 BackingPublicDto

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/BackingPublicDto.cs`

**Estructura:**
```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// DTO reducido para lista publica de backings.
/// NO expone informacion sensible del usuario (solo nombre o "Anonimo").
/// </summary>
public class BackingPublicDto
{
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre del backer o "Anonimo" si !PermitirMostrarNombre.
    /// Se resuelve en backend usando UserService o directamente "Anonimo".
    /// </summary>
    public string NombreBacker { get; set; } = null!;

    /// <summary>
    /// Monto del aporte.
    /// </summary>
    public decimal Monto { get; set; }

    /// <summary>
    /// Nombre del reward seleccionado (null si no hay reward).
    /// </summary>
    public string? RewardNombre { get; set; }

    /// <summary>
    /// Mensaje publico del backer (null si no hay mensaje).
    /// </summary>
    public string? Mensaje { get; set; }

    /// <summary>
    /// Fecha de creacion del backing.
    /// </summary>
    public DateTime FechaCreacion { get; set; }
}
```

**Mapeos desde:**
- `PedidoCrowdfunding`
- `NombreBacker`: Si `PermitirMostrarNombre = true` → buscar nombre via `UserService.GetUserNameAsync(UserId)`, sino → `"Anonimo"`

---

### 3.3 CampaniaDetailDto

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaDetailDto.cs`

**Estructura:**
```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// DTO extendido de campania con datos relacionados para vista publica.
/// Incluye rewards activos, backings recientes, y estadisticas calculadas.
/// </summary>
public class CampaniaDetailDto
{
    // ===== DATOS BASE DE CAMPANIA =====
    public Guid Id { get; set; }
    public Guid ArtistaId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }

    // ===== METRICAS Y PROGRESO =====
    public decimal ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public decimal ImportePledgedActual { get; set; }

    /// <summary>
    /// Porcentaje de progreso calculado: (ImportePledgedActual / ImporteObjetivo) * 100.
    /// Redondeado a 1 decimal.
    /// </summary>
    public decimal PorcentajeProgreso { get; set; }

    // ===== MONEDA Y ESTADO =====
    public int MonedaId { get; set; }
    public string MonedaSimbolo { get; set; } = null!;
    public int EstadoCampaniaId { get; set; }
    public string EstadoCampaniaNombre { get; set; } = null!;

    // ===== CONFIGURACION =====
    public bool PermiteAportacionesAnonimas { get; set; }
    public bool PermitePropinas { get; set; }

    // ===== FECHAS =====
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }

    /// <summary>
    /// Dias restantes hasta FechaFin calculados desde DateTime.UtcNow.
    /// Si FechaFin es null o ya paso, retorna 0.
    /// </summary>
    public int DiasRestantes { get; set; }

    // ===== DATOS DE ARTISTA =====
    public string ArtistaNombre { get; set; } = null!;
    public string? ArtistaImagenUrl { get; set; }

    // ===== REWARDS ACTIVOS =====
    /// <summary>
    /// Lista de rewards activos ordenados por Orden ASC.
    /// Solo incluye rewards con EsActivo = true.
    /// </summary>
    public List<RewardPublicDto> Rewards { get; set; } = new();

    // ===== BACKINGS RECIENTES =====
    /// <summary>
    /// Ultimos 10 backings (max) ordenados por FechaCreacion DESC.
    /// Solo incluye backings con EstadoPedidoId = 3 (COMPLETADO).
    /// </summary>
    public List<BackingPublicDto> BackingsRecientes { get; set; } = new();

    /// <summary>
    /// Total de backers unicos (count de PedidoCrowdfunding con EstadoPedidoId = 3).
    /// </summary>
    public int TotalBackers { get; set; }

    public DateTime FechaCreacion { get; set; }
}
```

**Mapeos desde:**
- `CampaniaCrowdfunding` (datos base)
- `Artista.NombreArtistico` → `ArtistaNombre`
- `Artista.ImagenPerfilUrl` → `ArtistaImagenUrl`
- `MaestraMoneda.Simbolo` → `MonedaSimbolo`
- `MaestraEstadoCampania.Nombre` → `EstadoCampaniaNombre`
- Calculos:
  - `PorcentajeProgreso = (ImportePledgedActual / ImporteObjetivo) * 100`
  - `DiasRestantes = Math.Max(0, (FechaFin - DateTime.UtcNow).Days)`
  - `TotalBackers = PedidoCrowdfunding.Count(p => p.EstadoPedidoId == 3)`

---

### 3.4 RewardPublicDto

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/RewardPublicDto.cs`

**Estructura:**
```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// DTO reducido de reward para vista publica.
/// Incluye campos calculados de disponibilidad y stock vendido.
/// </summary>
public class RewardPublicDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal ImporteMinimo { get; set; }
    public int? CantidadMaxima { get; set; }

    /// <summary>
    /// Cantidad vendida (calculada desde PedidoCrowdfundingLinea).
    /// Sum(Cantidad) donde EstadoPedidoId = 3 (COMPLETADO).
    /// </summary>
    public int CantidadVendida { get; set; }

    /// <summary>
    /// Si el reward esta disponible para seleccionar.
    /// True si CantidadMaxima == null OR CantidadVendida &lt; CantidadMaxima.
    /// </summary>
    public bool Disponible { get; set; }

    public bool IncluyeEnvioFisico { get; set; }
    public string? TiempoEntregaEstimado { get; set; }
    public int Orden { get; set; }
    public bool EsActivo { get; set; }
}
```

**Mapeos desde:**
- `CampaniaCrowdfundingReward`
- Calculos:
  - `CantidadVendida = PedidoCrowdfundingLinea.Where(l => l.RewardId == Id && l.PedidoCrowdfunding.EstadoPedidoId == 3).Sum(l => l.Cantidad)`
  - `Disponible = CantidadMaxima == null || CantidadVendida < CantidadMaxima`

---

### 3.5 CampaniaStatsDto

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaStatsDto.cs`

**Estructura:**
```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// DTO de estadisticas agregadas de backings de una campania.
/// Todos los calculos basados en PedidoCrowdfunding con EstadoPedidoId = 3 (COMPLETADO).
/// </summary>
public class CampaniaStatsDto
{
    public Guid CampaniaId { get; set; }

    /// <summary>
    /// Total de backers (count de PedidoCrowdfunding con EstadoPedidoId = 3).
    /// </summary>
    public int TotalBackers { get; set; }

    /// <summary>
    /// Total recaudado (sum de ImporteTotal de PedidoCrowdfunding con EstadoPedidoId = 3).
    /// Debe coincidir con CampaniaCrowdfunding.ImportePledgedActual.
    /// </summary>
    public decimal TotalRecaudado { get; set; }

    /// <summary>
    /// Promedio de aporte (avg de ImporteTotal).
    /// </summary>
    public decimal PromedioAporte { get; set; }

    /// <summary>
    /// Aporte minimo (min de ImporteTotal).
    /// 0 si no hay backings.
    /// </summary>
    public decimal AporteMinimo { get; set; }

    /// <summary>
    /// Aporte maximo (max de ImporteTotal).
    /// 0 si no hay backings.
    /// </summary>
    public decimal AporteMaximo { get; set; }

    /// <summary>
    /// Dias restantes hasta FechaFin calculados desde DateTime.UtcNow.
    /// </summary>
    public int DiasRestantes { get; set; }
}
```

**Calculos desde:**
- `PedidoCrowdfunding` filtrado por `CampaniaId` y `EstadoPedidoId = 3`
- `TotalBackers = pedidos.Count()`
- `TotalRecaudado = pedidos.Sum(p => p.ImporteTotal)`
- `PromedioAporte = pedidos.Average(p => p.ImporteTotal)` (o 0 si no hay pedidos)
- `AporteMinimo = pedidos.Min(p => p.ImporteTotal)` (o 0 si no hay pedidos)
- `AporteMaximo = pedidos.Max(p => p.ImporteTotal)` (o 0 si no hay pedidos)
- `DiasRestantes = Math.Max(0, (campania.FechaFin - DateTime.UtcNow).Days)`

---

## 4. Validadores

### 4.1 CreateBackingCommandValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Backings/Validators/CreateBackingCommandValidator.cs`

**Reglas de Validacion:**

| Campo | Regla | Mensaje | ErrorCode | Tipo |
|-------|-------|---------|-----------|------|
| CampaniaId | NotEmpty | El ID de campania es obligatorio | Validation_Required | Sincrona |
| Monto | GreaterThanOrEqualTo(1) | El monto debe ser al menos 1 EUR | Validation_InvalidAmount | Sincrona |
| Monto | Custom: verificar monto >= reward.ImporteMinimo (si RewardId != null) | El monto debe ser al menos {reward.ImporteMinimo} EUR para esta recompensa | BusinessRule_AmountBelowMinimum | Asincrona |
| Mensaje | MaximumLength(500) | El mensaje no puede superar los 500 caracteres | Validation_MaxLength | Sincrona |
| RewardId | Custom: si != null, validar que reward existe y es activo | Recompensa no encontrada | NotFound_Reward | Asincrona |
| RewardId | Custom: si != null, validar que reward tiene stock disponible | Esta recompensa esta agotada | BusinessRule_RewardOutOfStock | Asincrona |
| CampaniaId | Custom: validar que campania existe | Campania no encontrada | NotFound_Campania | Asincrona |
| CampaniaId | Custom: validar que campania.EstadoCampaniaId == 2 (PUBLICADA) | Esta campania no esta activa | BusinessRule_CampaniaNotActive | Asincrona |
| CampaniaId | Custom: validar que DateTime.UtcNow <= campania.FechaFin | Esta campania ya finalizo | BusinessRule_CampaniaEnded | Asincrona |
| UserId | Custom: si UserId == null y campania.PermiteAportacionesAnonimas == false | Debes iniciar sesion para hacer un aporte | Auth_UserNotAuthenticated | Asincrona |

**Dependencias Inyectadas:**
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

**Validaciones Asincronas - Pseudocodigo:**
```csharp
// Validar campania existe y esta activa
RuleFor(x => x.CampaniaId)
    .MustAsync(async (campaniaId, ct) =>
    {
        var campania = await _campaniaService.GetByIdAsync(campaniaId, ct);
        return campania != null;
    })
    .WithMessage("Campania no encontrada")
    .WithErrorCode(ServiceResponseMessageType.NotFound_Campania);

RuleFor(x => x)
    .MustAsync(async (command, ct) =>
    {
        var campania = await _campaniaService.GetByIdAsync(command.CampaniaId, ct);
        return campania?.EstadoCampaniaId == 2; // PUBLICADA
    })
    .WithMessage("Esta campania no esta activa")
    .WithErrorCode(ServiceResponseMessageType.BusinessRule_CampaniaNotActive);

// Validar reward existe, esta activo y tiene stock
When(x => x.RewardId.HasValue, () =>
{
    RuleFor(x => x.RewardId!.Value)
        .MustAsync(async (rewardId, ct) =>
        {
            var reward = await _rewardService.GetByIdAsync(rewardId, ct);
            return reward != null && reward.EsActivo;
        })
        .WithMessage("Recompensa no encontrada")
        .WithErrorCode(ServiceResponseMessageType.NotFound_Reward);

    RuleFor(x => x)
        .MustAsync(async (command, ct) =>
        {
            var reward = await _rewardService.GetByIdAsync(command.RewardId!.Value, ct);
            if (reward == null) return true; // Ya validado arriba

            // Calcular cantidad vendida
            var cantidadVendida = await _rewardService.GetCantidadVendidaAsync(reward.Id, ct);
            var disponible = reward.CantidadMaxima == null || cantidadVendida < reward.CantidadMaxima;

            return disponible;
        })
        .WithMessage("Esta recompensa esta agotada")
        .WithErrorCode(ServiceResponseMessageType.BusinessRule_RewardOutOfStock);

    RuleFor(x => x.Monto)
        .MustAsync(async (command, monto, ct) =>
        {
            var reward = await _rewardService.GetByIdAsync(command.RewardId!.Value, ct);
            return reward == null || monto >= reward.ImporteMinimo;
        })
        .WithMessage((command, monto) =>
            $"El monto debe ser al menos {/* obtener reward.ImporteMinimo */} EUR para esta recompensa")
        .WithErrorCode(ServiceResponseMessageType.BusinessRule_AmountBelowMinimum);
});

// Validar anonimos permitidos
RuleFor(x => x)
    .MustAsync(async (command, ct) =>
    {
        if (command.UserId != null) return true; // Usuario autenticado, OK

        var campania = await _campaniaService.GetByIdAsync(command.CampaniaId, ct);
        return campania?.PermiteAportacionesAnonimas == true;
    })
    .WithMessage("Debes iniciar sesion para hacer un aporte")
    .WithErrorCode(ServiceResponseMessageType.Auth_UserNotAuthenticated);
```

**Nota:** Las validaciones asincronas usan `IRequestCacheService` via Services para evitar queries duplicados (ver ADR-006).

---

### 4.2 GetBackingsByCampaniaQueryValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Backings/Validators/GetBackingsByCampaniaQueryValidator.cs`

**Reglas de Validacion:**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| CampaniaId | NotEmpty | El ID de campania es obligatorio | Validation_Required |
| PageNumber | GreaterThan(0) | El numero de pagina debe ser mayor a 0 | Validation_InvalidRange |
| PageSize | InclusiveBetween(1, 100) | El tamano de pagina debe estar entre 1 y 100 | Validation_InvalidRange |

**Dependencias:** Ninguna (solo validaciones sincronas).

---

### 4.3 GetCampaniaStatsQueryValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/GetCampaniaStatsQueryValidator.cs`

**Reglas de Validacion:**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| CampaniaId | NotEmpty | El ID de campania es obligatorio | Validation_Required |

**Dependencias:** Ninguna.

---

## 5. AutoMapper Mappings

### 5.1 BackingProfile

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/BackingProfile.cs`

**Mappings:**
```csharp
namespace WePlayRises.Crowdfunding.Application.Mapping;

public class BackingProfile : Profile
{
    public BackingProfile()
    {
        // Command -> PedidoCrowdfunding (para crear entidad)
        CreateMap<CreateBackingCommand, PedidoCrowdfunding>()
            .ForMember(dest => dest.ImporteSubtotal, opt => opt.MapFrom(src => src.Monto))
            .ForMember(dest => dest.ImporteTotal, opt => opt.MapFrom(src => src.Monto))
            .ForMember(dest => dest.ImportePropina, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.ImporteEnvio, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.ImporteImpuestos, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.ComentarioBacker, opt => opt.MapFrom(src => src.Mensaje))
            .ForMember(dest => dest.PermitirMostrarNombre, opt => opt.MapFrom(src => !src.EsAnonimo))
            .ForMember(dest => dest.EstadoPedidoId, opt => opt.MapFrom(src => 3)) // COMPLETADO (MVP)
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FanProfileId, opt => opt.Ignore())
            .ForMember(dest => dest.DireccionEnvioId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
            .ForMember(dest => dest.Campania, opt => opt.Ignore())
            .ForMember(dest => dest.Lineas, opt => opt.Ignore())
            .ForMember(dest => dest.Aportaciones, opt => opt.Ignore());

        // PedidoCrowdfunding -> BackingDto (para response)
        CreateMap<PedidoCrowdfunding, BackingDto>()
            .ForMember(dest => dest.Monto, opt => opt.MapFrom(src => src.ImporteTotal))
            .ForMember(dest => dest.Mensaje, opt => opt.MapFrom(src => src.ComentarioBacker))
            .ForMember(dest => dest.EsAnonimo, opt => opt.MapFrom(src => !src.PermitirMostrarNombre))
            .ForMember(dest => dest.CampaniaTitulo, opt => opt.MapFrom(src => src.Campania.Titulo))
            .ForMember(dest => dest.RewardId, opt => opt.MapFrom(src =>
                src.Lineas.FirstOrDefault(l => l.EsRewardPrincipal) != null
                    ? src.Lineas.First(l => l.EsRewardPrincipal).RewardId
                    : (Guid?)null))
            .ForMember(dest => dest.RewardNombre, opt => opt.Ignore()) // Resolver en Handler con query adicional
            .ForMember(dest => dest.MonedaSimbolo, opt => opt.Ignore()) // Resolver en Handler con lookup MaestraMoneda
            .ForMember(dest => dest.EstadoPedido, opt => opt.Ignore()) // Resolver en Handler con lookup MaestraEstadoPedido
            .ForMember(dest => dest.UserName, opt => opt.Ignore()); // Resolver en Handler con UserService o "Anonimo"

        // PedidoCrowdfunding -> BackingPublicDto (para lista publica)
        CreateMap<PedidoCrowdfunding, BackingPublicDto>()
            .ForMember(dest => dest.Monto, opt => opt.MapFrom(src => src.ImporteTotal))
            .ForMember(dest => dest.Mensaje, opt => opt.MapFrom(src => src.ComentarioBacker))
            .ForMember(dest => dest.NombreBacker, opt => opt.Ignore()) // Resolver en Handler
            .ForMember(dest => dest.RewardNombre, opt => opt.Ignore()); // Resolver en Handler
    }
}
```

**Notas:**
- Algunos campos no se pueden mapear directamente con AutoMapper y requieren logica custom en Handler:
  - `RewardNombre`: Requiere join o query adicional a `CampaniaCrowdfundingReward`.
  - `MonedaSimbolo`, `EstadoPedido`: Requieren lookups a tablas maestras.
  - `UserName`, `NombreBacker`: Requieren llamada a `UserService` o devolver `"Anonimo"`.

---

### 5.2 CampaniaProfile (extension)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/CampaniaProfile.cs`

**Mappings a AGREGAR:**
```csharp
// CampaniaCrowdfunding -> CampaniaDetailDto
CreateMap<CampaniaCrowdfunding, CampaniaDetailDto>()
    .ForMember(dest => dest.PorcentajeProgreso, opt => opt.MapFrom(src =>
        src.ImporteObjetivo > 0
            ? Math.Round((src.ImportePledgedActual / src.ImporteObjetivo) * 100, 1)
            : 0))
    .ForMember(dest => dest.DiasRestantes, opt => opt.MapFrom(src =>
        src.FechaFin.HasValue
            ? Math.Max(0, (src.FechaFin.Value - DateTime.UtcNow).Days)
            : 0))
    .ForMember(dest => dest.MonedaSimbolo, opt => opt.Ignore()) // Resolver en Handler
    .ForMember(dest => dest.EstadoCampaniaNombre, opt => opt.Ignore()) // Resolver en Handler
    .ForMember(dest => dest.ArtistaNombre, opt => opt.Ignore()) // Resolver en Handler con query a Artista
    .ForMember(dest => dest.ArtistaImagenUrl, opt => opt.Ignore()) // Resolver en Handler
    .ForMember(dest => dest.Rewards, opt => opt.Ignore()) // Resolver en Handler con query a Rewards
    .ForMember(dest => dest.BackingsRecientes, opt => opt.Ignore()) // Resolver en Handler con query a Pedidos
    .ForMember(dest => dest.TotalBackers, opt => opt.Ignore()); // Resolver en Handler con count
```

---

### 5.3 RewardProfile (extension)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/RewardProfile.cs`

**Mappings a AGREGAR:**
```csharp
// CampaniaCrowdfundingReward -> RewardPublicDto
CreateMap<CampaniaCrowdfundingReward, RewardPublicDto>()
    .ForMember(dest => dest.CantidadVendida, opt => opt.Ignore()) // Resolver en Handler con calculo
    .ForMember(dest => dest.Disponible, opt => opt.Ignore()); // Resolver en Handler con calculo
```

---

## 6. OpenAPI Documentation

### 6.1 POST /api/campanias/{id}/backings

**Summary:** Crear nuevo backing (aporte) a una campania

**Request:**
- **Path Parameter:** `id` (Guid) - ID de la campania
- **Headers:**
  - `Authorization: Bearer {token}` (opcional, si usuario autenticado)
- **Body (application/json):**
```json
{
  "rewardId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "monto": 25.00,
  "mensaje": "Mucha suerte con el proyecto!",
  "esAnonimo": false
}
```

**Responses:**
- **201 Created:**
```json
{
  "data": {
    "id": "guid",
    "campaniaId": "guid",
    "campaniaTitulo": "Mi Album Debut",
    "rewardId": "guid",
    "rewardNombre": "CD Fisico Firmado",
    "monto": 25.00,
    "monedaSimbolo": "EUR",
    "mensaje": "Mucha suerte con el proyecto!",
    "esAnonimo": false,
    "userName": "Maria Lopez",
    "fechaCreacion": "2026-02-13T10:30:00Z",
    "estadoPedido": "Completado"
  },
  "messages": [
    {
      "message": "Aporte realizado con exito. Gracias por tu apoyo!",
      "errorCode": "0001"
    }
  ]
}
```

- **400 Bad Request:** (Validacion fallida)
```json
{
  "messages": [
    {
      "message": "El monto debe ser al menos 10 EUR para esta recompensa",
      "errorCode": "4012"
    }
  ]
}
```

- **401 Unauthorized:** (No autenticado y campania no permite anonimos)
```json
{
  "messages": [
    {
      "message": "Debes iniciar sesion para hacer un aporte",
      "errorCode": "3005"
    }
  ]
}
```

- **404 Not Found:** (Campania o reward no encontrado)
```json
{
  "messages": [
    {
      "message": "Campania no encontrada",
      "errorCode": "2003"
    }
  ]
}
```

- **409 Conflict:** (Campania no activa o reward agotado)
```json
{
  "messages": [
    {
      "message": "Esta recompensa esta agotada",
      "errorCode": "4011"
    }
  ]
}
```

- **500 Internal Server Error:**
```json
{
  "messages": [
    {
      "message": "Error inesperado al procesar el aporte",
      "errorCode": "5000"
    }
  ]
}
```

**Auth:** Opcional (Bearer JWT). Si no hay token, se permite backing anonimo si `campania.PermiteAportacionesAnonimas = true`.

---

### 6.2 GET /api/campanias/{id}

**Summary:** Obtener detalle completo de campania con rewards y backings recientes

**Request:**
- **Path Parameter:** `id` (Guid) - ID de la campania

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "artistaId": "guid",
    "titulo": "Mi Album Debut",
    "subtitulo": "Rock alternativo desde Madrid",
    "descripcionCorta": "Un viaje musical de 10 canciones...",
    "videoPrincipalUrl": "https://youtube.com/watch?v=...",
    "imagenPrincipalUrl": "https://...",
    "importeObjetivo": 5000.00,
    "importeMinimo": 1000.00,
    "importePledgedActual": 2340.00,
    "porcentajeProgreso": 46.8,
    "monedaId": 1,
    "monedaSimbolo": "EUR",
    "estadoCampaniaId": 2,
    "estadoCampaniaNombre": "Publicada",
    "permiteAportacionesAnonimas": true,
    "permitePropinas": true,
    "fechaInicio": "2026-02-01T00:00:00Z",
    "fechaFin": "2026-03-31T23:59:59Z",
    "diasRestantes": 46,
    "artistaNombre": "Juan Perez Music",
    "artistaImagenUrl": "https://...",
    "rewards": [ /* RewardPublicDto array */ ],
    "backingsRecientes": [ /* BackingPublicDto array */ ],
    "totalBackers": 234,
    "fechaCreacion": "2026-01-15T12:00:00Z"
  },
  "messages": [
    {
      "message": "Campania encontrada",
      "errorCode": "0000"
    }
  ]
}
```

**Response 404 Not Found:**
```json
{
  "messages": [
    {
      "message": "Campania no encontrada",
      "errorCode": "2003"
    }
  ]
}
```

**Auth:** No requerido (publico).

---

### 6.3 GET /api/campanias/{id}/backings

**Summary:** Listar backings publicos de una campania (paginado)

**Request:**
- **Path Parameter:** `id` (Guid) - ID de la campania
- **Query Parameters:**
  - `pageNumber` (int, default 1) - Numero de pagina
  - `pageSize` (int, default 20) - Elementos por pagina

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "guid",
        "nombreBacker": "Maria Lopez",
        "monto": 25.00,
        "rewardNombre": "CD Fisico Firmado",
        "mensaje": "Mucha suerte con el proyecto!",
        "fechaCreacion": "2026-02-13T10:30:00Z"
      }
    ],
    "totalCount": 234,
    "page": 1,
    "pageSize": 20,
    "totalPages": 12
  },
  "messages": [
    {
      "message": "Aportes encontrados",
      "errorCode": "0000"
    }
  ]
}
```

**Auth:** No requerido (publico).

---

### 6.4 GET /api/campanias/{id}/stats

**Summary:** Obtener estadisticas agregadas de backings de una campania

**Request:**
- **Path Parameter:** `id` (Guid) - ID de la campania

**Response 200 OK:**
```json
{
  "data": {
    "campaniaId": "guid",
    "totalBackers": 234,
    "totalRecaudado": 2340.00,
    "promedioAporte": 10.00,
    "aporteMinimo": 1.00,
    "aporteMaximo": 100.00,
    "diasRestantes": 46
  },
  "messages": [
    {
      "message": "Estadisticas obtenidas",
      "errorCode": "0000"
    }
  ]
}
```

**Auth:** No requerido (publico).

---

## 7. Nuevas Constantes de Error

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Constants/ServiceResponseMessageType.cs`

**Constantes a AGREGAR:**
```csharp
// =========================================================================
// BUSINESS RULE ERRORS (4000-4999) - AGREGAR estas 3 nuevas constantes
// =========================================================================
public const string BusinessRule_RewardOutOfStock = "4011";
public const string BusinessRule_AmountBelowMinimum = "4012";
public const string BusinessRule_AnonymousNotAllowed = "4013";
```

**Notas:**
- Las constantes `BusinessRule_CampaniaNotActive = "4006"` y `BusinessRule_CampaniaEnded = "4007"` YA EXISTEN en el archivo actual.
- `NotFound_Reward = "2004"` y `NotFound_Backing = "2005"` YA EXISTEN.
- `Auth_UserNotAuthenticated = "3005"` YA EXISTE.

---

## 8. Archivos a Crear

```
Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
├── Features/
│   ├── Backings/                              # NUEVO directorio
│   │   ├── Commands/
│   │   │   └── CreateBackingCommand.cs        # Command + Handler
│   │   ├── Queries/
│   │   │   └── GetBackingsByCampaniaQuery.cs  # Query + Handler
│   │   └── Validators/
│   │       ├── CreateBackingCommandValidator.cs
│   │       └── GetBackingsByCampaniaQueryValidator.cs
│   └── Campanias/
│       ├── Queries/
│       │   ├── GetCampaniaDetailQuery.cs      # NUEVO - Query + Handler
│       │   └── GetCampaniaStatsQuery.cs       # NUEVO - Query + Handler
│       └── Validators/
│           ├── GetCampaniaDetailQueryValidator.cs  # NUEVO (opcional, solo valida ID)
│           └── GetCampaniaStatsQueryValidator.cs   # NUEVO (opcional, solo valida ID)
├── Dtos/
│   ├── BackingDto.cs                          # NUEVO
│   ├── BackingPublicDto.cs                    # NUEVO
│   ├── CampaniaDetailDto.cs                   # NUEVO
│   ├── CampaniaStatsDto.cs                    # NUEVO
│   └── RewardPublicDto.cs                     # NUEVO
└── Mapping/
    ├── BackingProfile.cs                      # NUEVO
    ├── CampaniaProfile.cs                     # MODIFICAR (agregar mappings)
    └── RewardProfile.cs                       # MODIFICAR (agregar mappings)
```

**Archivos a MODIFICAR:**
- `ServiceResponseMessageType.cs` - Agregar 3 constantes nuevas (4011, 4012, 4013)
- `CampaniaProfile.cs` - Agregar mapping `CampaniaCrowdfunding -> CampaniaDetailDto`
- `RewardProfile.cs` - Agregar mapping `CampaniaCrowdfundingReward -> RewardPublicDto`

---

## 9. Flujo de Creacion de Backing (Handler Logic)

### CreateBackingCommandHandler - Pseudocodigo

```csharp
public async Task<ServiceResponse<BackingDto>> Handle(
    CreateBackingCommand request,
    CancellationToken cancellationToken)
{
    try
    {
        // 1. VALIDACION
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ServiceResponse<BackingDto>
            {
                Messages = validationResult.GetServiceResponseMessages()
            };
        }

        // 2. CARGAR DATOS NECESARIOS (usar cache via Services)
        var campania = await _campaniaService.GetByIdAsync(request.CampaniaId, cancellationToken);
        var reward = request.RewardId.HasValue
            ? await _rewardService.GetByIdAsync(request.RewardId.Value, cancellationToken)
            : null;

        // 3. MAPEAR COMMAND -> ENTIDAD
        var pedido = _mapper.Map<PedidoCrowdfunding>(request);
        pedido.MonedaId = campania.MonedaId;

        // 4. CREAR TRANSACCION ATOMICA
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 4.1. Crear PedidoCrowdfunding
            await _context.PedidoCrowdfunding.AddAsync(pedido, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // 4.2. Crear PedidoCrowdfundingLinea
            var linea = new PedidoCrowdfundingLinea
            {
                PedidoId = pedido.Id,
                RewardId = request.RewardId,
                Cantidad = 1,
                PrecioUnitario = request.Monto,
                ImporteLinea = request.Monto,
                EsRewardPrincipal = true
            };
            await _context.PedidoCrowdfundingLinea.AddAsync(linea, cancellationToken);

            // 4.3. Actualizar ImportePledgedActual de campania
            campania.ImportePledgedActual += request.Monto;
            _context.CampaniaCrowdfunding.Update(campania);

            // 4.4. Crear AportacionCrowdfunding simulada (MVP)
            var aportacion = new AportacionCrowdfunding
            {
                PedidoId = pedido.Id,
                EstadoAportacionId = 2, // CONFIRMADO (simulado)
                MetodoPagoId = 99, // MVP_SIMULADO
                ImporteAportacion = request.Monto,
                MonedaId = campania.MonedaId,
                FechaAportacion = DateTime.UtcNow
            };
            await _context.AportacionCrowdfunding.AddAsync(aportacion, cancellationToken);

            // 4.5. Guardar cambios y commitear transaccion
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error en transaccion de backing para campania {CampaniaId}", request.CampaniaId);
            throw;
        }

        // 5. MAPEAR ENTIDAD -> DTO Y RESOLVER CAMPOS CUSTOM
        var dto = _mapper.Map<BackingDto>(pedido);
        dto.CampaniaTitulo = campania.Titulo;
        dto.RewardNombre = reward?.Nombre;
        dto.MonedaSimbolo = "EUR"; // Lookup desde MaestraMoneda via campaniaService
        dto.EstadoPedido = "Completado"; // Lookup desde MaestraEstadoPedido
        dto.UserName = request.EsAnonimo || string.IsNullOrEmpty(request.UserId)
            ? "Anonimo"
            : request.UserName;

        // 6. RETORNAR RESPUESTA EXITOSA
        return new ServiceResponse<BackingDto>
        {
            Data = dto,
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Aporte realizado con exito. Gracias por tu apoyo!",
                    ErrorCode = ServiceResponseMessageType.Created
                }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creando backing para campania {CampaniaId}", request.CampaniaId);
        return new ServiceResponse<BackingDto>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Error inesperado al procesar el aporte",
                    ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
                }
            }
        };
    }
}
```

**Dependencias del Handler:**
```csharp
private readonly ICampaniaService _campaniaService;
private readonly IRewardService _rewardService;
private readonly CrowdfundingContext _context; // Para transaccion
private readonly IMapper _mapper;
private readonly IValidator<CreateBackingCommand> _validator;
private readonly ILogger<CreateBackingCommandHandler> _logger;

public CreateBackingCommandHandler(
    ICampaniaService campaniaService,
    IRewardService rewardService,
    CrowdfundingContext context,
    IMapper mapper,
    IValidator<CreateBackingCommand> validator,
    ILogger<CreateBackingCommandHandler> logger)
{
    _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
    _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
    _context = context ?? throw new ArgumentNullException(nameof(context));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**NOTA CRITICA:** El Handler EXCEPCIONALMENTE inyecta `CrowdfundingContext` para poder usar transacciones explicitas. Esta es una excepcion justificada porque el backing involucra multiples operaciones atomicas:
1. Crear `PedidoCrowdfunding`
2. Crear `PedidoCrowdfundingLinea`
3. Actualizar `CampaniaCrowdfunding.ImportePledgedActual`
4. Crear `AportacionCrowdfunding`

Estas 4 operaciones deben ser atomicas (todo o nada). Los Services individuales NO pueden garantizar atomicidad entre entidades diferentes.

**Alternativa (si se prefiere NO inyectar DbContext en Handler):**
- Crear un `IBackingService` con metodo `CreateBackingAsync` que encapsule toda la logica transaccional.
- El Handler solo llama a ese service y hace el mapping del resultado.

---

## 10. Checklist de Implementacion

- [ ] **Constantes:** Agregar 3 nuevas constantes (4011, 4012, 4013) a `ServiceResponseMessageType.cs`
- [ ] **DTOs:** Crear 5 nuevos DTOs (BackingDto, BackingPublicDto, CampaniaDetailDto, RewardPublicDto, CampaniaStatsDto)
- [ ] **Commands:** Crear `CreateBackingCommand.cs` con Command + Handler en MISMO archivo
- [ ] **Queries:** Crear 3 queries (GetCampaniaDetailQuery, GetBackingsByCampaniaQuery, GetCampaniaStatsQuery) con Query + Handler
- [ ] **Validators:** Crear 2 validators con FluentValidation + ServiceResponseMessageType constants
- [ ] **Mappings:** Crear `BackingProfile.cs` y extender `CampaniaProfile.cs` y `RewardProfile.cs`
- [ ] **Handler Logic:** Implementar transaccion atomica en `CreateBackingCommandHandler`
- [ ] **Controller:** Agregar endpoint POST /api/campanias/{id}/backings en `CampaniasController`
- [ ] **Controller:** Agregar endpoint GET /api/campanias/{id}/backings en `CampaniasController`
- [ ] **Controller:** Agregar endpoint GET /api/campanias/{id}/stats en `CampaniasController`
- [ ] **Controller:** Modificar endpoint GET /api/campanias/{id} para usar `GetCampaniaDetailQuery` (o crear nuevo endpoint)
- [ ] **Services:** Agregar metodo `GetCantidadVendidaAsync(Guid rewardId)` a `IRewardService` para calcular stock vendido
- [ ] **Services:** Considerar agregar `IBackingService` para encapsular logica transaccional (alternativa a inyectar DbContext)
- [ ] **Tests:** Unit tests para Validator (todas las reglas)
- [ ] **Tests:** Unit tests para Handler (happy path + errores)
- [ ] **Tests:** Integration tests para endpoint POST /backings (transaccion atomica)

---

## 11. Reglas CQRS Aplicadas

- ✓ **Regla 1:** Handler + Command en MISMO archivo (`CreateBackingCommand.cs`)
- ✓ **Regla 2:** SIEMPRE retornar `ServiceResponse<T>` (todos los handlers)
- ⚠️ **Regla 3:** Handler NUNCA inyecta DbContext - **EXCEPCION:** `CreateBackingCommandHandler` inyecta `CrowdfundingContext` para transacciones atomicas. Alternativa: Crear `IBackingService` que encapsule la transaccion.
- ✓ **Regla 4:** Services retornan Entidades, NO DTOs
- ✓ **Regla 5:** Validators con `.WithMessage()` Y `.WithErrorCode(ServiceResponseMessageType.X)`
- ✓ **Regla 6:** Constructor con `?? throw new ArgumentNullException` para TODAS las dependencias
- ✓ **Regla 7:** Validacion retorna ServiceResponse, NO throw
- ✓ **Regla 8:** Caching via Services (usar `IRequestCacheService` en validators)
- ✓ **Regla 9:** Try-Catch con logging en handlers + ServiceResponseMessageType constants

---

## 12. Proximos Pasos

Despues de implementar estos contratos API:

1. **Backend Implementation:**
   - Crear los 5 DTOs nuevos
   - Crear los Commands/Queries con sus Handlers
   - Crear los Validators con reglas completas
   - Crear los AutoMapper Profiles
   - Actualizar Controllers con nuevos endpoints
   - Agregar metodo `GetCantidadVendidaAsync` a `RewardService`

2. **Frontend Integration:**
   - Usar los tipos TypeScript de `plans/hacer-backing/shared/contracts-plan.md`
   - Crear hooks `useCreateBacking`, `useCampaniaDetail`, `useCampaniaStats`
   - Crear componentes de UI (CampaniaCard, RewardCard, BackingForm, etc.)

3. **Testing:**
   - Unit tests para validators (todas las reglas)
   - Unit tests para handlers (mocking services)
   - Integration tests para transaccion atomica de backing
   - E2E tests para flujo completo desde landing

---

**Fin del plan de contratos API para hacer-backing.**
