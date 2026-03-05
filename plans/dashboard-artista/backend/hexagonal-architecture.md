# Arquitectura Hexagonal: Dashboard de Artista

**Fecha:** 2026-02-14
**Modulo:** Crowdfunding + UserAccess
**Feature:** dashboard-artista (US-05)

## 1. Resumen Ejecutivo

El Dashboard de Artista proporciona una vista consolidada de metricas clave para que los artistas monitoreen sus campañas de crowdfunding. Esta feature implementa queries de solo lectura optimizadas que calculan estadisticas en tiempo real (total recaudado, numero de backers, progreso de campanias, distribucion de rewards) sin modificar entidades existentes. Se centra en la capa de servicios agregados y queries CQRS especializadas para el dashboard.

## 2. Domain Layer

### 2.1 Entidades Existentes (NO MODIFICAR)

Las siguientes entidades YA EXISTEN y se reutilizan sin cambios:

#### CampaniaCrowdfunding
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Model/CampaniaCrowdfunding.cs`

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | CampaniaCrowdfundingId | No | PK |
| ArtistaId | ArtistaId | No | FK a Artista |
| Titulo | string | No | Nombre de la campania |
| EstadoCampaniaId | int | No | 1=Borrador, 2=Publicada, 3=Finalizada, 4=Cancelada |
| ImporteObjetivo | decimal | No | Meta financiera |
| ImportePledgedActual | decimal | No | Total recaudado actual |
| FechaInicio | DateTime? | Si | Fecha de lanzamiento |
| FechaFin | DateTime? | Si | Fecha de cierre |
| FechaCreacion | DateTime | No | Fecha de creacion |

**Navegaciones:**
- `Pedidos` -> `ICollection<PedidoCrowdfunding>` (1:N)
- `Rewards` -> `ICollection<CampaniaCrowdfundingReward>` (1:N)

#### PedidoCrowdfunding
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Model/PedidoCrowdfunding.cs`

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | PedidoCrowdfundingId | No | PK |
| CampaniaId | CampaniaCrowdfundingId | No | FK a Campania |
| UserId | string? | Si | FK a Identity User (null si anonimo) |
| EstadoPedidoId | int | No | 1=Pendiente, 3=Completado |
| ImporteTotal | decimal | No | Monto total del backing |
| PermitirMostrarNombre | bool | No | Si el backer permite mostrar su identidad |
| ComentarioBacker | string? | Si | Mensaje del backer al artista |
| FechaCreacion | DateTime | No | Fecha del backing |

**Navegaciones:**
- `Campania` -> `CampaniaCrowdfunding`
- `Lineas` -> `ICollection<PedidoCrowdfundingLinea>` (1:N)

#### Artista
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Model/Artista.cs`

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | ArtistaId | No | PK |
| UserIdPropietario | string | No | FK a Identity User |
| NombreArtistico | string | No | Nombre artistico |
| FechaCreacion | DateTime | No | Fecha de registro |

### 2.2 Constants (Sin Cambios)

**IMPORTANTE:** NO se requieren nuevos ErrorCodes. Se reutilizan los existentes en:

```csharp
// Ubicacion: Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Constants/ServiceResponseMessageType.cs
public static class ServiceResponseMessageType
{
    // Success (0000-0999)
    public const string Success = "0000";

    // NotFound (2000-2999)
    public const string NotFound_Campania = "2003";

    // Auth (3000-3999)
    public const string Auth_Forbidden = "3002";

    // Internal (5000-5999)
    public const string Internal_UnexpectedError = "5000";
}
```

```csharp
// Ubicacion: Modules/UserAccess/WePlayRises.UserAccess.Domain/Constants/ServiceResponseMessageType.cs
public static class ServiceResponseMessageType
{
    // NotFound (2000-2999)
    public const string NotFound_Artista = "2002";
}
```

### 2.3 Repository Interfaces (NO REQUIERE NUEVOS)

Las interfaces existentes YA cubren las necesidades del dashboard:

**ICampaniaRepository** (Existente)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Repositories/ICampaniaRepository.cs`

- `GetByIdAsync(CampaniaCrowdfundingId, ct)` - Obtener campania
- `GetByArtistaIdAsync(ArtistaId, ct)` - Listar campanias del artista
- `CountBackersByCampaniaIdAsync(CampaniaCrowdfundingId, ct)` - Contar backers

**IPedidoRepository** (Existente)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Repositories/IPedidoRepository.cs`

- Metodos base de CRUD para acceso a pedidos

**IArtistaRepository** (Existente)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Interfaces/IArtistaRepository.cs`

- `GetByUserIdAsync(string userId, ct)` - Obtener artista por UserId

---

## 3. Infrastructure Layer

### 3.1 Repository Extensions (NUEVOS METODOS)

#### ICampaniaRepository - Extensiones
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Repositories/ICampaniaRepository.cs`

**AGREGAR los siguientes metodos a la interfaz existente:**

```csharp
// Metodos NUEVOS para dashboard
Task<IReadOnlyList<CampaniaCrowdfunding>> GetMisCampaniasPaginatedAsync(
    ArtistaId artistaId,
    int? estadoCampaniaId,
    int page,
    int pageSize,
    CancellationToken ct);

Task<int> CountMisCampaniasAsync(
    ArtistaId artistaId,
    int? estadoCampaniaId,
    CancellationToken ct);
```

#### CampaniaRepository - Implementacion
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Repositories/CampaniaRepository.cs`

**AGREGAR implementacion al repositorio existente:**

```csharp
public async Task<IReadOnlyList<CampaniaCrowdfunding>> GetMisCampaniasPaginatedAsync(
    ArtistaId artistaId,
    int? estadoCampaniaId,
    int page,
    int pageSize,
    CancellationToken ct)
{
    var query = _context.Campanias
        .AsNoTracking()
        .Where(c => c.ArtistaId == artistaId && !c.Borrado);

    if (estadoCampaniaId.HasValue)
    {
        query = query.Where(c => c.EstadoCampaniaId == estadoCampaniaId.Value);
    }

    return await query
        .OrderByDescending(c => c.FechaCreacion)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);
}

public async Task<int> CountMisCampaniasAsync(
    ArtistaId artistaId,
    int? estadoCampaniaId,
    CancellationToken ct)
{
    var query = _context.Campanias
        .Where(c => c.ArtistaId == artistaId && !c.Borrado);

    if (estadoCampaniaId.HasValue)
    {
        query = query.Where(c => c.EstadoCampaniaId == estadoCampaniaId.Value);
    }

    return await query.CountAsync(ct);
}
```

#### IPedidoRepository - Extensiones
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Repositories/IPedidoRepository.cs`

**AGREGAR los siguientes metodos a la interfaz existente:**

```csharp
// Metodos NUEVOS para dashboard
Task<IReadOnlyList<PedidoCrowdfunding>> GetByCampaniaIdPaginatedAsync(
    CampaniaCrowdfundingId campaniaId,
    int page,
    int pageSize,
    CancellationToken ct);

Task<int> CountByCampaniaIdAsync(
    CampaniaCrowdfundingId campaniaId,
    CancellationToken ct);

Task<Dictionary<Guid, int>> GetRewardStatsByCampaniaIdAsync(
    CampaniaCrowdfundingId campaniaId,
    CancellationToken ct);

Task<PedidoCrowdfunding?> GetLastByCampaniaIdAsync(
    CampaniaCrowdfundingId campaniaId,
    CancellationToken ct);

Task<IReadOnlyList<(DateTime Date, int Count, decimal Total)>> GetProgressByDayAsync(
    CampaniaCrowdfundingId campaniaId,
    CancellationToken ct);
```

#### PedidoRepository - Implementacion
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Repositories/PedidoRepository.cs`

**AGREGAR implementacion al repositorio existente:**

```csharp
public async Task<IReadOnlyList<PedidoCrowdfunding>> GetByCampaniaIdPaginatedAsync(
    CampaniaCrowdfundingId campaniaId,
    int page,
    int pageSize,
    CancellationToken ct)
{
    return await _context.Pedidos
        .AsNoTracking()
        .Include(p => p.Lineas)
            .ThenInclude(l => l.Reward)
        .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3)
        .OrderByDescending(p => p.FechaCreacion)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);
}

public async Task<int> CountByCampaniaIdAsync(
    CampaniaCrowdfundingId campaniaId,
    CancellationToken ct)
{
    return await _context.Pedidos
        .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3)
        .CountAsync(ct);
}

public async Task<Dictionary<Guid, int>> GetRewardStatsByCampaniaIdAsync(
    CampaniaCrowdfundingId campaniaId,
    CancellationToken ct)
{
    return await _context.PedidoCrowdfundingLineas
        .Where(l => l.PedidoCrowdfunding.CampaniaId == campaniaId
                 && l.PedidoCrowdfunding.EstadoPedidoId == 3
                 && l.RewardId.HasValue)
        .GroupBy(l => l.RewardId.Value)
        .Select(g => new { RewardId = g.Key, Count = g.Sum(l => l.Cantidad) })
        .ToDictionaryAsync(x => x.RewardId, x => x.Count, ct);
}

public async Task<PedidoCrowdfunding?> GetLastByCampaniaIdAsync(
    CampaniaCrowdfundingId campaniaId,
    CancellationToken ct)
{
    return await _context.Pedidos
        .AsNoTracking()
        .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3)
        .OrderByDescending(p => p.FechaCreacion)
        .FirstOrDefaultAsync(ct);
}

public async Task<IReadOnlyList<(DateTime Date, int Count, decimal Total)>> GetProgressByDayAsync(
    CampaniaCrowdfundingId campaniaId,
    CancellationToken ct)
{
    return await _context.Pedidos
        .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3)
        .GroupBy(p => p.FechaCreacion.Date)
        .OrderBy(g => g.Key)
        .Select(g => new ValueTuple<DateTime, int, decimal>(
            g.Key,
            g.Count(),
            g.Sum(p => p.ImporteTotal)
        ))
        .ToListAsync(ct);
}
```

### 3.2 Services (NUEVOS)

#### IDashboardService (NUEVO)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Services/IDashboardService.cs`

```csharp
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<(decimal TotalRecaudado, int TotalBackers, int CampaniasActivas, int CampaniasCompletadas)>
        GetResumenAsync(ArtistaId artistaId, CancellationToken ct);

    Task<decimal> GetBackingPromedioAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);

    Task<string?> GetRewardMasPopularAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);

    Task<decimal> GetVelocidadDiariaAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);

    Task<decimal?> CalcularProyeccionFinalAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
}
```

#### DashboardService (NUEVA IMPLEMENTACION)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Services/DashboardService.cs`

```csharp
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Infra.Context;

namespace WePlayRises.Crowdfunding.Infra.Services;

public class DashboardService : IDashboardService
{
    private readonly ICampaniaRepository _campaniaRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IRewardRepository _rewardRepository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<DashboardService> _logger;
    private readonly CrowdfundingContext _context;

    public DashboardService(
        ICampaniaRepository campaniaRepository,
        IPedidoRepository pedidoRepository,
        IRewardRepository rewardRepository,
        IRequestCacheService requestCache,
        ILogger<DashboardService> logger,
        CrowdfundingContext context)
    {
        _campaniaRepository = campaniaRepository ?? throw new ArgumentNullException(nameof(campaniaRepository));
        _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
        _rewardRepository = rewardRepository ?? throw new ArgumentNullException(nameof(rewardRepository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<(decimal TotalRecaudado, int TotalBackers, int CampaniasActivas, int CampaniasCompletadas)>
        GetResumenAsync(ArtistaId artistaId, CancellationToken ct)
    {
        var campanias = await _campaniaRepository.GetByArtistaIdAsync(artistaId, ct);

        var totalRecaudado = campanias.Sum(c => c.ImportePledgedActual);

        // Obtener backers unicos contando pedidos completados
        var campaniaIds = campanias.Select(c => c.Id).ToList();
        var totalBackers = await _context.Pedidos
            .Where(p => campaniaIds.Contains(p.CampaniaId) && p.EstadoPedidoId == 3)
            .Select(p => p.UserId ?? p.Id.ToString())
            .Distinct()
            .CountAsync(ct);

        var campaniasActivas = campanias.Count(c => c.EstadoCampaniaId == 2);
        var campaniasCompletadas = campanias.Count(c => c.EstadoCampaniaId == 3);

        return (totalRecaudado, totalBackers, campaniasActivas, campaniasCompletadas);
    }

    public async Task<decimal> GetBackingPromedioAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var campania = await _campaniaRepository.GetByIdAsync(campaniaId, ct);
        if (campania == null) return 0;

        var totalBackers = await _pedidoRepository.CountByCampaniaIdAsync(campaniaId, ct);
        if (totalBackers == 0) return 0;

        return Math.Round(campania.ImportePledgedActual / totalBackers, 2);
    }

    public async Task<string?> GetRewardMasPopularAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var rewardStats = await _pedidoRepository.GetRewardStatsByCampaniaIdAsync(campaniaId, ct);
        if (!rewardStats.Any()) return null;

        var rewardMasVendidoId = rewardStats.OrderByDescending(x => x.Value).First().Key;
        var reward = await _rewardRepository.GetByIdAsync(
            new CampaniaCrowdfundingRewardId(rewardMasVendidoId), ct);

        return reward?.Nombre;
    }

    public async Task<decimal> GetVelocidadDiariaAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var campania = await _campaniaRepository.GetByIdAsync(campaniaId, ct);
        if (campania?.FechaInicio == null) return 0;

        var diasTranscurridos = (DateTime.UtcNow - campania.FechaInicio.Value).Days;
        if (diasTranscurridos <= 0) return 0;

        return Math.Round(campania.ImportePledgedActual / diasTranscurridos, 2);
    }

    public async Task<decimal?> CalcularProyeccionFinalAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var campania = await _campaniaRepository.GetByIdAsync(campaniaId, ct);
        if (campania?.FechaInicio == null || campania.FechaFin == null) return null;

        var velocidadDiaria = await GetVelocidadDiariaAsync(campaniaId, ct);
        var diasRestantes = Math.Max(0, (campania.FechaFin.Value - DateTime.UtcNow).Days);

        return Math.Round(campania.ImportePledgedActual + (velocidadDiaria * diasRestantes), 2);
    }
}
```

**IMPORTANTE - Service DEBE tener:**
- `IRequestCacheService` para cache (evita queries duplicados)
- `ILogger<DashboardService>` para logging
- `?? throw new ArgumentNullException` en TODAS las dependencias

### 3.3 Dependency Injection

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/DependencyInjection.cs`

**AGREGAR registro del nuevo servicio:**

```csharp
services.AddScoped<IDashboardService, DashboardService>();
```

---

## 4. Application Layer (QUERIES NUEVAS)

### 4.1 DTOs (NUEVOS)

#### DashboardResumenDto
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/DashboardResumenDto.cs`

```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

public class DashboardResumenDto
{
    public Guid ArtistaId { get; set; }
    public string NombreArtistico { get; set; } = null!;
    public decimal TotalRecaudado { get; set; }
    public int TotalBackers { get; set; }
    public int CampaniasActivas { get; set; }
    public int CampaniasCompletadas { get; set; }
    public int TotalCampanias { get; set; }
    public string MonedaSimbolo { get; set; } = "EUR";
    public DateTime? FechaUltimoAporte { get; set; }
}
```

#### MiCampaniaListItemDto
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/MiCampaniaListItemDto.cs`

```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

public class MiCampaniaListItemDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? ImagenPrincipalUrl { get; set; }
    public int EstadoCampaniaId { get; set; }
    public string EstadoCampaniaNombre { get; set; } = null!;
    public decimal ImporteObjetivo { get; set; }
    public decimal ImporteRecaudado { get; set; }
    public decimal PorcentajeProgreso { get; set; }
    public int NumBackers { get; set; }
    public int? DiasRestantes { get; set; }
    public DateTime? FechaFin { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

#### CampaniaBackingListDto
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaBackingListDto.cs`

```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

public class CampaniaBackingListDto
{
    public Guid CampaniaId { get; set; }
    public string CampaniaTitulo { get; set; } = null!;
    public CampaniaBackingStatsDto Stats { get; set; } = null!;
    public PaginatedResponse<CampaniaBackingItemDto> Backings { get; set; } = null!;
}

public class CampaniaBackingStatsDto
{
    public decimal TotalRecaudado { get; set; }
    public decimal BackingPromedio { get; set; }
    public int TotalBackers { get; set; }
    public string? RewardMasPopular { get; set; }
    public UltimoBackingDto? UltimoBacking { get; set; }
}

public class UltimoBackingDto
{
    public string NombreBacker { get; set; } = null!;
    public decimal Monto { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class CampaniaBackingItemDto
{
    public Guid Id { get; set; }
    public string NombreBacker { get; set; } = null!;
    public string? Email { get; set; }
    public decimal Monto { get; set; }
    public string? RewardNombre { get; set; }
    public string? Mensaje { get; set; }
    public bool EsAnonimo { get; set; }
    public string EstadoPedido { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
```

#### CampaniaStatsDetailDto
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaStatsDetailDto.cs`

```csharp
namespace WePlayRises.Crowdfunding.Application.Dtos;

public class CampaniaStatsDetailDto
{
    public Guid CampaniaId { get; set; }
    public string CampaniaTitulo { get; set; } = null!;
    public decimal ImporteObjetivo { get; set; }
    public decimal ImporteRecaudado { get; set; }
    public decimal PorcentajeProgreso { get; set; }
    public int NumBackers { get; set; }
    public decimal BackingPromedio { get; set; }
    public int? DiasRestantes { get; set; }
    public int DiasTranscurridos { get; set; }
    public int TotalDiasCampania { get; set; }
    public decimal? ProyeccionFinal { get; set; }
    public decimal VelocidadDiaria { get; set; }
    public List<RewardStatDto> RewardStats { get; set; } = new();
    public List<ProgressoDiaDto> ProgressoPorDia { get; set; } = new();
}

public class RewardStatDto
{
    public Guid? RewardId { get; set; }
    public string RewardNombre { get; set; } = null!;
    public int CantidadVendida { get; set; }
    public decimal TotalRecaudado { get; set; }
    public decimal PorcentajeDelTotal { get; set; }
}

public class ProgressoDiaDto
{
    public string Fecha { get; set; } = null!;
    public int NumBackings { get; set; }
    public decimal TotalRecaudado { get; set; }
    public decimal Acumulado { get; set; }
}
```

### 4.2 Queries (NUEVAS)

#### GetDashboardResumenQuery
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Dashboard/Queries/GetDashboardResumenQuery.cs`

```csharp
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Application.Responses;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdfunding.Application.Features.Dashboard.Queries;

public class GetDashboardResumenQuery : IRequest<ServiceResponse<DashboardResumenDto>>
{
    public string UserId { get; set; } = null!;
}

public class GetDashboardResumenQueryHandler
    : IRequestHandler<GetDashboardResumenQuery, ServiceResponse<DashboardResumenDto>>
{
    private readonly IArtistaService _artistaService;
    private readonly IDashboardService _dashboardService;
    private readonly ICampaniaRepository _campaniaRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly ILogger<GetDashboardResumenQueryHandler> _logger;

    public GetDashboardResumenQueryHandler(
        IArtistaService artistaService,
        IDashboardService dashboardService,
        ICampaniaRepository campaniaRepository,
        IPedidoRepository pedidoRepository,
        ILogger<GetDashboardResumenQueryHandler> logger)
    {
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        _campaniaRepository = campaniaRepository ?? throw new ArgumentNullException(nameof(campaniaRepository));
        _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<DashboardResumenDto>> Handle(
        GetDashboardResumenQuery request,
        CancellationToken ct)
    {
        try
        {
            // 1. Validar que el usuario tiene perfil de Artista
            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            if (artista == null)
            {
                return new ServiceResponse<DashboardResumenDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Artista no encontrado",
                            ErrorCode = UserAccess.Domain.Constants.ServiceResponseMessageType.NotFound_Artista
                        }
                    }
                };
            }

            // 2. Obtener metricas agregadas
            var (totalRecaudado, totalBackers, campaniasActivas, campaniasCompletadas) =
                await _dashboardService.GetResumenAsync(artista.Id, ct);

            // 3. Obtener fecha del ultimo aporte
            var campanias = await _campaniaRepository.GetByArtistaIdAsync(artista.Id, ct);
            var totalCampanias = campanias.Count;

            PedidoCrowdfunding? ultimoPedido = null;
            foreach (var campania in campanias)
            {
                var ultimo = await _pedidoRepository.GetLastByCampaniaIdAsync(campania.Id, ct);
                if (ultimo != null && (ultimoPedido == null || ultimo.FechaCreacion > ultimoPedido.FechaCreacion))
                {
                    ultimoPedido = ultimo;
                }
            }

            // 4. Construir respuesta
            var dto = new DashboardResumenDto
            {
                ArtistaId = artista.Id.Value,
                NombreArtistico = artista.NombreArtistico,
                TotalRecaudado = totalRecaudado,
                TotalBackers = totalBackers,
                CampaniasActivas = campaniasActivas,
                CampaniasCompletadas = campaniasCompletadas,
                TotalCampanias = totalCampanias,
                MonedaSimbolo = "EUR",
                FechaUltimoAporte = ultimoPedido?.FechaCreacion
            };

            return new ServiceResponse<DashboardResumenDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Resumen obtenido", ErrorCode = ServiceResponseMessageType.Success }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo resumen de dashboard para UserId {UserId}", request.UserId);
            return new ServiceResponse<DashboardResumenDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Error inesperado", ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError }
                }
            };
        }
    }
}
```

#### GetMisCampaniasQuery (REEMPLAZA QUERY EXISTENTE)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetMisCampaniasQuery.cs`

**NOTA:** Esta query YA EXISTE pero debe EXTENDERSE para soportar paginacion y calculos de metricas.

```csharp
// Agregar propiedades de paginacion al Query existente
public class GetMisCampaniasQuery : IRequest<ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>>
{
    public string UserId { get; set; } = null!;
    public int? EstadoCampaniaId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// Handler debe calcular metricas por campania
// - PorcentajeProgreso = (ImportePledgedActual / ImporteObjetivo) * 100
// - NumBackers = CountBackersByCampaniaIdAsync()
// - DiasRestantes = MAX(0, (FechaFin - Today).Days)
```

#### GetCampaniaBackingsQuery (NUEVA)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaBackingsQuery.cs`

```csharp
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Application.Responses;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;

public class GetCampaniaBackingsQuery : IRequest<ServiceResponse<CampaniaBackingListDto>>
{
    public Guid CampaniaId { get; set; }
    public string UserId { get; set; } = null!;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetCampaniaBackingsQueryHandler
    : IRequestHandler<GetCampaniaBackingsQuery, ServiceResponse<CampaniaBackingListDto>>
{
    private readonly IArtistaService _artistaService;
    private readonly ICampaniaService _campaniaService;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<GetCampaniaBackingsQueryHandler> _logger;

    public GetCampaniaBackingsQueryHandler(
        IArtistaService artistaService,
        ICampaniaService campaniaService,
        IPedidoRepository pedidoRepository,
        IDashboardService dashboardService,
        ILogger<GetCampaniaBackingsQueryHandler> logger)
    {
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
        _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<CampaniaBackingListDto>> Handle(
        GetCampaniaBackingsQuery request,
        CancellationToken ct)
    {
        try
        {
            // 1. Validar artista
            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            if (artista == null)
            {
                return new ServiceResponse<CampaniaBackingListDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new() { Message = "Artista no encontrado", ErrorCode = UserAccess.Domain.Constants.ServiceResponseMessageType.NotFound_Artista }
                    }
                };
            }

            // 2. Validar campania
            var campaniaId = new CampaniaCrowdfundingId(request.CampaniaId);
            var campania = await _campaniaService.GetByIdAsync(campaniaId, ct);
            if (campania == null)
            {
                return new ServiceResponse<CampaniaBackingListDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new() { Message = "Campania no encontrada", ErrorCode = ServiceResponseMessageType.NotFound_Campania }
                    }
                };
            }

            // 3. Validar ownership
            if (campania.ArtistaId != artista.Id)
            {
                return new ServiceResponse<CampaniaBackingListDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new() { Message = "No tienes permiso para ver estos aportes", ErrorCode = ServiceResponseMessageType.Auth_Forbidden }
                    }
                };
            }

            // 4. Obtener pedidos paginados
            var pedidos = await _pedidoRepository.GetByCampaniaIdPaginatedAsync(
                campaniaId, request.Page, request.PageSize, ct);
            var totalCount = await _pedidoRepository.CountByCampaniaIdAsync(campaniaId, ct);

            // 5. Mapear a DTOs (anonimizar si necesario)
            var backingItems = pedidos.Select(p => new CampaniaBackingItemDto
            {
                Id = p.Id.Value,
                NombreBacker = p.PermitirMostrarNombre ? "Usuario" : "Anonimo", // Obtener nombre real via Identity
                Email = p.PermitirMostrarNombre ? null : null, // Obtener email via Identity
                Monto = p.ImporteTotal,
                RewardNombre = p.Lineas.FirstOrDefault()?.Reward?.Nombre,
                Mensaje = p.ComentarioBacker,
                EsAnonimo = !p.PermitirMostrarNombre,
                EstadoPedido = "Completado",
                FechaCreacion = p.FechaCreacion
            }).ToList();

            // 6. Calcular stats
            var backingPromedio = await _dashboardService.GetBackingPromedioAsync(campaniaId, ct);
            var rewardMasPopular = await _dashboardService.GetRewardMasPopularAsync(campaniaId, ct);
            var ultimoPedido = await _pedidoRepository.GetLastByCampaniaIdAsync(campaniaId, ct);

            var stats = new CampaniaBackingStatsDto
            {
                TotalRecaudado = campania.ImportePledgedActual,
                BackingPromedio = backingPromedio,
                TotalBackers = totalCount,
                RewardMasPopular = rewardMasPopular,
                UltimoBacking = ultimoPedido != null ? new UltimoBackingDto
                {
                    NombreBacker = ultimoPedido.PermitirMostrarNombre ? "Usuario" : "Anonimo",
                    Monto = ultimoPedido.ImporteTotal,
                    FechaCreacion = ultimoPedido.FechaCreacion
                } : null
            };

            // 7. Construir respuesta paginada
            var response = new CampaniaBackingListDto
            {
                CampaniaId = campania.Id.Value,
                CampaniaTitulo = campania.Titulo,
                Stats = stats,
                Backings = new PaginatedResponse<CampaniaBackingItemDto>
                {
                    Items = backingItems,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                }
            };

            return new ServiceResponse<CampaniaBackingListDto>
            {
                Data = response,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Aportes encontrados", ErrorCode = ServiceResponseMessageType.Success }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo backings de campania {CampaniaId}", request.CampaniaId);
            return new ServiceResponse<CampaniaBackingListDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Error inesperado", ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError }
                }
            };
        }
    }
}
```

#### GetCampaniaStatsDetailQuery (NUEVA)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaStatsDetailQuery.cs`

```csharp
// Similar estructura a GetCampaniaBackingsQuery
// Retorna: ServiceResponse<CampaniaStatsDetailDto>
// Calcula: porcentaje, dias restantes/transcurridos, proyeccion, reward stats, progreso por dia
```

### 4.3 Validators (NUEVOS)

#### GetMisCampaniasQueryValidator
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/GetMisCampaniasQueryValidator.cs`

```csharp
using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Validators;

public class GetMisCampaniasQueryValidator : AbstractValidator<GetMisCampaniasQuery>
{
    public GetMisCampaniasQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId es requerido")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page debe ser mayor o igual a 1")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidValue);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize debe estar entre 1 y 100")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidValue);

        RuleFor(x => x.EstadoCampaniaId)
            .InclusiveBetween(1, 5)
            .When(x => x.EstadoCampaniaId.HasValue)
            .WithMessage("EstadoCampaniaId invalido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidValue);
    }
}
```

#### GetCampaniaBackingsQueryValidator
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/GetCampaniaBackingsQueryValidator.cs`

```csharp
using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Validators;

public class GetCampaniaBackingsQueryValidator : AbstractValidator<GetCampaniaBackingsQuery>
{
    public GetCampaniaBackingsQueryValidator()
    {
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("CampaniaId es requerido")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId es requerido")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page debe ser mayor o igual a 1")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidValue);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize debe estar entre 1 y 100")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidValue);
    }
}
```

---

## 5. Entity Framework Core - Optimizaciones

### 5.1 Indices (AGREGAR SI NO EXISTEN)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Context/CrowdfundingContext.cs`

```csharp
// En OnModelCreating, agregar indices para dashboard
modelBuilder.Entity<CampaniaCrowdfunding>()
    .HasIndex(c => c.ArtistaId);

modelBuilder.Entity<CampaniaCrowdfunding>()
    .HasIndex(c => new { c.ArtistaId, c.EstadoCampaniaId });

modelBuilder.Entity<PedidoCrowdfunding>()
    .HasIndex(p => new { p.CampaniaId, p.EstadoPedidoId });

modelBuilder.Entity<PedidoCrowdfunding>()
    .HasIndex(p => p.FechaCreacion);
```

### 5.2 Proyecciones Optimizadas

Para queries de solo lectura, usar proyecciones directas:

```csharp
// EVITAR - Cargar entidades completas
var campanias = await _context.Campanias.ToListAsync();
var dtos = _mapper.Map<List<MiCampaniaListItemDto>>(campanias);

// PREFERIR - Proyeccion directa
var campanias = await _context.Campanias
    .AsNoTracking()
    .Where(c => c.ArtistaId == artistaId)
    .Select(c => new MiCampaniaListItemDto
    {
        Id = c.Id.Value,
        Titulo = c.Titulo,
        ImporteObjetivo = c.ImporteObjetivo,
        ImporteRecaudado = c.ImportePledgedActual,
        // ... proyectar solo lo necesario
    })
    .ToListAsync();
```

---

## 6. Estructura de Archivos

```
Modules/Crowdfunding/
├── WePlayRises.Crowdfunding.Domain/
│   ├── Constants/
│   │   └── ServiceResponseMessageType.cs (EXISTENTE - NO MODIFICAR)
│   └── Model/ (EXISTENTE - NO MODIFICAR)
│
├── WePlayRises.Crowdfunding.Application/
│   ├── Dtos/ (NUEVOS)
│   │   ├── DashboardResumenDto.cs
│   │   ├── MiCampaniaListItemDto.cs
│   │   ├── CampaniaBackingListDto.cs
│   │   └── CampaniaStatsDetailDto.cs
│   │
│   ├── Features/
│   │   ├── Dashboard/ (NUEVO)
│   │   │   └── Queries/
│   │   │       └── GetDashboardResumenQuery.cs
│   │   │
│   │   └── Campanias/
│   │       ├── Queries/ (EXTENDER EXISTENTE)
│   │       │   ├── GetMisCampaniasQuery.cs (EXTENDER)
│   │       │   ├── GetCampaniaBackingsQuery.cs (NUEVO)
│   │       │   └── GetCampaniaStatsDetailQuery.cs (NUEVO)
│   │       │
│   │       └── Validators/ (NUEVOS)
│   │           ├── GetMisCampaniasQueryValidator.cs
│   │           ├── GetCampaniaBackingsQueryValidator.cs
│   │           └── GetCampaniaStatsDetailQueryValidator.cs
│   │
│   └── Interfaces/
│       ├── Repositories/
│       │   ├── ICampaniaRepository.cs (AGREGAR METODOS)
│       │   └── IPedidoRepository.cs (AGREGAR METODOS)
│       │
│       └── Services/
│           └── IDashboardService.cs (NUEVO)
│
└── WePlayRises.Crowdfunding.Infra/
    ├── Repositories/
    │   ├── CampaniaRepository.cs (AGREGAR METODOS)
    │   └── PedidoRepository.cs (AGREGAR METODOS)
    │
    ├── Services/
    │   └── DashboardService.cs (NUEVO)
    │
    └── DependencyInjection.cs (AGREGAR REGISTRO)
```

---

## 7. Checklist

- [ ] Entidades NO se modifican (solo se consultan)
- [ ] Repository interfaces extendidas con metodos de dashboard
- [ ] Repository implementations con queries optimizados (AsNoTracking, indices)
- [ ] **IDashboardService creado en Application/Interfaces/Services**
- [ ] **DashboardService implementado en Infra/Services**
- [ ] **DashboardService inyecta UnitOfWork + Repositories + Cache + Logger**
- [ ] **DashboardService usa `?? throw new ArgumentNullException` en constructor**
- [ ] Services retornan entidades o tipos primitivos (no DTOs)
- [ ] Queries retornan ServiceResponse<DTO>
- [ ] Validators con Message + ErrorCode (constants)
- [ ] Handlers con try-catch + logging
- [ ] DI registrado correctamente (DashboardService)
- [ ] Indices en tablas para queries de dashboard
- [ ] Proyecciones directas para queries de solo lectura

---

## 8. Puntos Clave de Implementacion

### 8.1 Calculo de Metricas

**PorcentajeProgreso:**
```csharp
var porcentaje = campania.ImporteObjetivo > 0
    ? Math.Round((campania.ImportePledgedActual / campania.ImporteObjetivo) * 100, 2)
    : 0;
```

**DiasRestantes:**
```csharp
var diasRestantes = campania.FechaFin.HasValue
    ? Math.Max(0, (campania.FechaFin.Value - DateTime.UtcNow).Days)
    : (int?)null;
```

**BackingPromedio:**
```csharp
var backingPromedio = totalBackers > 0
    ? Math.Round(importeRecaudado / totalBackers, 2)
    : 0;
```

### 8.2 Autorizacion en Queries

**CRITICO:** Validar ownership en TODOS los queries de campania especifica:

```csharp
// 1. Obtener artista del usuario autenticado
var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
if (artista == null) return NotFound_Artista;

// 2. Obtener campania
var campania = await _campaniaService.GetByIdAsync(campaniaId, ct);
if (campania == null) return NotFound_Campania;

// 3. Validar ownership
if (campania.ArtistaId != artista.Id) return Auth_Forbidden;
```

### 8.3 Anonimizacion de Backings

```csharp
var nombreBacker = pedido.PermitirMostrarNombre
    ? await GetUserNameAsync(pedido.UserId) // Llamar a Identity
    : "Anonimo";

var email = pedido.PermitirMostrarNombre && pedido.UserId != null
    ? await GetUserEmailAsync(pedido.UserId)
    : null;
```

### 8.4 Caching de Artista

El artista se puede cachear por request para evitar multiples queries:

```csharp
var artista = await _requestCache.GetOrAddAsync(
    $"artista:user:{userId}",
    async () => await _artistaService.GetByUserIdAsync(userId, ct));
```

---

## 9. Dependencias Externas

### 9.1 Modulo UserAccess

**Requerido:**
- `IArtistaService.GetByUserIdAsync()` - Validar artista

**NO requerido:**
- No se modifica ninguna entidad de UserAccess
- No se crean nuevos repositorios en UserAccess

### 9.2 Identity (ASP.NET Core)

**Requerido para obtener nombre/email de backers:**
- `UserManager<ApplicationUser>.FindByIdAsync(userId)`
- `UserManager<ApplicationUser>.GetEmailAsync(user)`

**Integracion en Handler:**
```csharp
// Inyectar UserManager en Handler
private readonly UserManager<ApplicationUser> _userManager;

// Obtener nombre del usuario
var user = await _userManager.FindByIdAsync(pedido.UserId);
var nombre = user?.UserName ?? "Usuario";
```

---

## 10. Migraciones

**NO SE REQUIEREN MIGRACIONES** para esta feature porque:
- No se agregan nuevas entidades
- No se modifican entidades existentes
- Solo se agregan indices (opcional, pueden agregarse via migration separada)

**Opcional - Agregar indices:**
```bash
dotnet ef migrations add AddDashboardIndices --project src/api/Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra
dotnet ef database update --project src/api/Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra
```

---

## 11. Siguiente Paso

Una vez aprobado este plan, el siguiente paso es:
1. **Implementar repositorio extensions** (metodos nuevos en CampaniaRepository y PedidoRepository)
2. **Crear DashboardService** (nuevo servicio con calculos de metricas)
3. **Implementar queries CQRS** (GetDashboardResumenQuery, GetCampaniaBackingsQuery, etc.)
4. **Crear validators** para cada query
5. **Registrar DI** para DashboardService
6. **Crear unit tests** para DashboardService y Handlers

**Nota:** Los DTOs del punto 4.1 deben crearse ANTES de implementar las queries.

---

**Fin del plan arquitectonico.**
