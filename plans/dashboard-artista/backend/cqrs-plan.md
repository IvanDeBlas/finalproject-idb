# Plan CQRS: Dashboard de Artista

**Fecha:** 2026-02-14
**Modulo:** Crowdfunding
**Feature:** dashboard-artista

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Request | Response |
|-----------|------|---------|----------|
| Obtener resumen dashboard | Query | GetDashboardResumenQuery | ServiceResponse&lt;DashboardResumenDto&gt; |
| Listar mis campanias | Query | GetMisCampaniasQuery | ServiceResponse&lt;PaginatedResponse&lt;MiCampaniaListItemDto&gt;&gt; |
| Listar backings de campania | Query | GetCampaniaBackingsQuery | ServiceResponse&lt;CampaniaBackingListDto&gt; |
| Obtener stats de campania | Query | GetCampaniaStatsQuery | ServiceResponse&lt;CampaniaStatsDetailDto&gt; |

**IMPORTANTE:** Todas las operaciones son QUERIES (solo lectura). No se modifican entidades.

---

## 2. Queries

### 2.1 GetDashboardResumenQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Dashboard/Queries/GetDashboardResumenQuery.cs`

**Contiene:** Query + Handler (MISMO archivo)

#### Query

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| UserId | string | ID del usuario autenticado (inyectado desde JWT claim "sub") |

**Implementa:** `IRequest<ServiceResponse<DashboardResumenDto>>`

**No requiere validator** - UserId se valida en Handler (que el usuario tenga perfil de Artista).

#### Handler

**Dependencias:**
- `IArtistaService` - Para obtener artista por UserId
- `IDashboardService` - Para calcular metricas agregadas
- `ICampaniaRepository` - Para obtener campanias del artista
- `IPedidoRepository` - Para obtener ultimo aporte
- `ILogger<GetDashboardResumenQueryHandler>` - Para logging

**Constructor con ?? throw:**
```csharp
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
```

**Flujo:**
1. **Validar artista**: Llamar `_artistaService.GetByUserIdAsync(request.UserId, ct)`
   - Si `artista == null`, retornar `ServiceResponse` con error `NotFound_Artista` (UserAccess.Domain.Constants)
2. **Obtener metricas agregadas**: Llamar `_dashboardService.GetResumenAsync(artista.Id, ct)`
   - Retorna tupla: `(totalRecaudado, totalBackers, campaniasActivas, campaniasCompletadas)`
3. **Obtener total campanias**: Llamar `_campaniaRepository.GetByArtistaIdAsync(artista.Id, ct)` y contar items
4. **Obtener fecha ultimo aporte**:
   - Iterar campanias y llamar `_pedidoRepository.GetLastByCampaniaIdAsync(campania.Id, ct)` para cada una
   - Comparar `FechaCreacion` y retornar el mas reciente
5. **Construir DTO**: Mapear manualmente (no usar AutoMapper)
6. **Retornar ServiceResponse exitoso** con `ErrorCode = ServiceResponseMessageType.Success`
7. **Try-catch**: Capturar excepciones, log con `_logger.LogError()`, retornar `Internal_UnexpectedError`

**Codigo ejemplo (Handler completo):**
```csharp
public async Task<ServiceResponse<DashboardResumenDto>> Handle(
    GetDashboardResumenQuery request,
    CancellationToken ct)
{
    try
    {
        // 1. Validar artista
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

        // 3. Obtener total campanias
        var campanias = await _campaniaRepository.GetByArtistaIdAsync(artista.Id, ct);
        var totalCampanias = campanias.Count;

        // 4. Obtener fecha ultimo aporte
        PedidoCrowdfunding? ultimoPedido = null;
        foreach (var campania in campanias)
        {
            var ultimo = await _pedidoRepository.GetLastByCampaniaIdAsync(campania.Id, ct);
            if (ultimo != null && (ultimoPedido == null || ultimo.FechaCreacion > ultimoPedido.FechaCreacion))
            {
                ultimoPedido = ultimo;
            }
        }

        // 5. Construir DTO
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

        // 6. Retornar ServiceResponse exitoso
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
        // 7. Try-catch con logging
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
```

---

### 2.2 GetMisCampaniasQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetMisCampaniasQuery.cs`

**Contiene:** Query + Handler (MISMO archivo)

#### Query

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| UserId | string | ID del usuario autenticado |
| EstadoCampaniaId | int? | Filtro por estado (1=Borrador, 2=Publicada, 3=Finalizada, 4=Cancelada, 5=Pausada) |
| Page | int | Numero de pagina (default: 1) |
| PageSize | int | Items por pagina (default: 10) |

**Implementa:** `IRequest<ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>>`

#### Handler

**Dependencias:**
- `IArtistaService` - Para obtener artista por UserId
- `ICampaniaRepository` - Para obtener campanias paginadas
- `IPedidoRepository` - Para contar backers por campania
- `IValidator<GetMisCampaniasQuery>` - Para validacion de parametros
- `ILogger<GetMisCampaniasQueryHandler>` - Para logging

**Constructor con ?? throw:**
```csharp
public GetMisCampaniasQueryHandler(
    IArtistaService artistaService,
    ICampaniaRepository campaniaRepository,
    IPedidoRepository pedidoRepository,
    IValidator<GetMisCampaniasQuery> validator,
    ILogger<GetMisCampaniasQueryHandler> logger)
{
    _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
    _campaniaRepository = campaniaRepository ?? throw new ArgumentNullException(nameof(campaniaRepository));
    _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Flujo:**
1. **Validar parametros**: Ejecutar `_validator.ValidateAsync(request, ct)`
   - Si invalido, retornar `ServiceResponse` con errores de validacion
2. **Validar artista**: Llamar `_artistaService.GetByUserIdAsync(request.UserId, ct)`
   - Si `artista == null`, retornar error `NotFound_Artista`
3. **Obtener campanias paginadas**: Llamar `_campaniaRepository.GetMisCampaniasPaginatedAsync(artista.Id, estadoCampaniaId, page, pageSize, ct)`
4. **Obtener total count**: Llamar `_campaniaRepository.CountMisCampaniasAsync(artista.Id, estadoCampaniaId, ct)`
5. **Mapear a DTOs**: Para cada campania:
   - Calcular `PorcentajeProgreso = Math.Round((c.ImportePledgedActual / c.ImporteObjetivo) * 100, 2)`
   - Obtener `NumBackers = _pedidoRepository.CountByCampaniaIdAsync(c.Id, ct)`
   - Calcular `DiasRestantes = c.FechaFin.HasValue ? Math.Max(0, (c.FechaFin.Value - DateTime.UtcNow).Days) : null`
6. **Construir PaginatedResponse**:
   - `Items = misCampaniasDto`
   - `TotalCount = totalCount`
   - `Page = request.Page`
   - `PageSize = request.PageSize`
   - `TotalPages = (totalCount + pageSize - 1) / pageSize`
7. **Retornar ServiceResponse exitoso**
8. **Try-catch con logging**

**Calculos clave:**
```csharp
var dto = new MiCampaniaListItemDto
{
    Id = campania.Id.Value,
    Titulo = campania.Titulo,
    ImagenPrincipalUrl = campania.ImagenPrincipalUrl,
    EstadoCampaniaId = campania.EstadoCampaniaId,
    EstadoCampaniaNombre = GetEstadoNombre(campania.EstadoCampaniaId),
    ImporteObjetivo = campania.ImporteObjetivo,
    ImporteRecaudado = campania.ImportePledgedActual,
    PorcentajeProgreso = campania.ImporteObjetivo > 0
        ? Math.Round((campania.ImportePledgedActual / campania.ImporteObjetivo) * 100, 2)
        : 0,
    NumBackers = await _pedidoRepository.CountByCampaniaIdAsync(campania.Id, ct),
    DiasRestantes = campania.FechaFin.HasValue
        ? Math.Max(0, (campania.FechaFin.Value - DateTime.UtcNow).Days)
        : (int?)null,
    FechaFin = campania.FechaFin,
    FechaCreacion = campania.FechaCreacion
};
```

**Helper para nombres de estado:**
```csharp
private static string GetEstadoNombre(int estadoId)
{
    return estadoId switch
    {
        1 => "Borrador",
        2 => "Publicada",
        3 => "Finalizada",
        4 => "Cancelada",
        5 => "Pausada",
        _ => "Desconocido"
    };
}
```

---

### 2.3 GetCampaniaBackingsQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaBackingsQuery.cs`

**Contiene:** Query + Handler (MISMO archivo)

#### Query

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| CampaniaId | Guid | ID de la campania (route parameter) |
| UserId | string | ID del usuario autenticado (para validar ownership) |
| Page | int | Numero de pagina (default: 1) |
| PageSize | int | Items por pagina (default: 20) |

**Implementa:** `IRequest<ServiceResponse<CampaniaBackingListDto>>`

#### Handler

**Dependencias:**
- `IArtistaService` - Para obtener artista por UserId
- `ICampaniaService` - Para obtener campania y validar ownership
- `IPedidoRepository` - Para obtener pedidos paginados
- `IDashboardService` - Para calcular stats (backing promedio, reward mas popular)
- `IValidator<GetCampaniaBackingsQuery>` - Para validacion
- `ILogger<GetCampaniaBackingsQueryHandler>` - Para logging

**Constructor con ?? throw:**
```csharp
public GetCampaniaBackingsQueryHandler(
    IArtistaService artistaService,
    ICampaniaService campaniaService,
    IPedidoRepository pedidoRepository,
    IDashboardService dashboardService,
    IValidator<GetCampaniaBackingsQuery> validator,
    ILogger<GetCampaniaBackingsQueryHandler> logger)
{
    _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
    _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
    _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
    _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Flujo:**
1. **Validar parametros**: Ejecutar `_validator.ValidateAsync(request, ct)`
2. **Validar artista**: Llamar `_artistaService.GetByUserIdAsync(request.UserId, ct)`
   - Si `artista == null`, retornar error `NotFound_Artista`
3. **Validar campania**: Llamar `_campaniaService.GetByIdAsync(new CampaniaCrowdfundingId(request.CampaniaId), ct)`
   - Si `campania == null`, retornar error `NotFound_Campania`
4. **Validar ownership**: Comparar `campania.ArtistaId != artista.Id`
   - Si NO coincide, retornar error `Auth_Forbidden` con mensaje "No tienes permiso para ver estos aportes"
5. **Obtener pedidos paginados**: Llamar `_pedidoRepository.GetByCampaniaIdPaginatedAsync(campaniaId, page, pageSize, ct)`
   - **IMPORTANTE**: Repository debe hacer `Include(p => p.Lineas).ThenInclude(l => l.Reward)` para obtener reward nombre
6. **Obtener total count**: Llamar `_pedidoRepository.CountByCampaniaIdAsync(campaniaId, ct)`
7. **Mapear a CampaniaBackingItemDto**: Para cada pedido:
   - **Anonimizar si necesario**: Si `!pedido.PermitirMostrarNombre` -> `NombreBacker = "Anonimo"`, `Email = null`
   - Si `PermitirMostrarNombre && pedido.UserId != null` -> Obtener nombre/email de Identity (UserManager)
   - `RewardNombre = pedido.Lineas.FirstOrDefault()?.Reward?.Nombre` (si tiene reward)
8. **Calcular stats**:
   - `BackingPromedio = await _dashboardService.GetBackingPromedioAsync(campaniaId, ct)`
   - `RewardMasPopular = await _dashboardService.GetRewardMasPopularAsync(campaniaId, ct)`
   - `UltimoPedido = await _pedidoRepository.GetLastByCampaniaIdAsync(campaniaId, ct)`
9. **Construir CampaniaBackingListDto** con stats y backings paginados
10. **Retornar ServiceResponse exitoso**
11. **Try-catch con logging**

**Logica de anonimizacion (CRITICO):**
```csharp
var backingItems = pedidos.Select(p => new CampaniaBackingItemDto
{
    Id = p.Id.Value,
    NombreBacker = p.PermitirMostrarNombre
        ? GetUserName(p.UserId) // Obtener de Identity
        : "Anonimo",
    Email = p.PermitirMostrarNombre && !string.IsNullOrEmpty(p.UserId)
        ? GetUserEmail(p.UserId) // Obtener de Identity
        : null,
    Monto = p.ImporteTotal,
    RewardNombre = p.Lineas.FirstOrDefault()?.Reward?.Nombre,
    Mensaje = p.ComentarioBacker,
    EsAnonimo = !p.PermitirMostrarNombre,
    EstadoPedido = "Completado", // Mapear desde EstadoPedidoId
    FechaCreacion = p.FechaCreacion
}).ToList();
```

**Nota sobre Identity:** El Handler puede inyectar `UserManager<ApplicationUser>` para obtener nombre/email de usuarios autenticados:
```csharp
private readonly UserManager<ApplicationUser> _userManager;

private string GetUserName(string? userId)
{
    if (string.IsNullOrEmpty(userId)) return "Backer";
    var user = _userManager.FindByIdAsync(userId).Result;
    return user?.UserName ?? "Backer";
}

private string? GetUserEmail(string? userId)
{
    if (string.IsNullOrEmpty(userId)) return null;
    var user = _userManager.FindByIdAsync(userId).Result;
    return user?.Email;
}
```

---

### 2.4 GetCampaniaStatsQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaStatsQuery.cs`

**Contiene:** Query + Handler (MISMO archivo)

#### Query

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| CampaniaId | Guid | ID de la campania |
| UserId | string | ID del usuario autenticado (para validar ownership) |

**Implementa:** `IRequest<ServiceResponse<CampaniaStatsDetailDto>>`

#### Handler

**Dependencias:**
- `IArtistaService` - Para obtener artista
- `ICampaniaService` - Para obtener campania
- `IPedidoRepository` - Para obtener progreso por dia y reward stats
- `IDashboardService` - Para calcular proyeccion final, velocidad diaria
- `IRewardRepository` - Para obtener nombres de rewards
- `IValidator<GetCampaniaStatsQuery>` - Para validacion
- `ILogger<GetCampaniaStatsQueryHandler>` - Para logging

**Constructor con ?? throw:**
```csharp
public GetCampaniaStatsQueryHandler(
    IArtistaService artistaService,
    ICampaniaService campaniaService,
    IPedidoRepository pedidoRepository,
    IDashboardService dashboardService,
    IRewardRepository rewardRepository,
    IValidator<GetCampaniaStatsQuery> validator,
    ILogger<GetCampaniaStatsQueryHandler> logger)
{
    _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
    _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
    _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
    _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
    _rewardRepository = rewardRepository ?? throw new ArgumentNullException(nameof(rewardRepository));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Flujo:**
1. **Validar parametros**
2. **Validar artista**
3. **Validar campania**
4. **Validar ownership** (igual que GetCampaniaBackingsQuery)
5. **Calcular metricas basicas**:
   - `PorcentajeProgreso = Math.Round((campania.ImportePledgedActual / campania.ImporteObjetivo) * 100, 2)`
   - `NumBackers = await _pedidoRepository.CountByCampaniaIdAsync(campaniaId, ct)`
   - `BackingPromedio = await _dashboardService.GetBackingPromedioAsync(campaniaId, ct)`
   - `DiasRestantes = campania.FechaFin.HasValue ? Math.Max(0, (campania.FechaFin.Value - DateTime.UtcNow).Days) : null`
   - `DiasTranscurridos = campania.FechaInicio.HasValue ? (DateTime.UtcNow - campania.FechaInicio.Value).Days : 0`
   - `TotalDiasCampania = (campania.FechaFin.HasValue && campania.FechaInicio.HasValue) ? (campania.FechaFin.Value - campania.FechaInicio.Value).Days : 0`
6. **Calcular metricas avanzadas**:
   - `VelocidadDiaria = await _dashboardService.GetVelocidadDiariaAsync(campaniaId, ct)`
   - `ProyeccionFinal = await _dashboardService.CalcularProyeccionFinalAsync(campaniaId, ct)`
7. **Obtener stats por reward**:
   - Llamar `_pedidoRepository.GetRewardStatsByCampaniaIdAsync(campaniaId, ct)` -> Dictionary<Guid, int>
   - Para cada rewardId, obtener nombre con `_rewardRepository.GetByIdAsync()`
   - Calcular `PorcentajeDelTotal = (cantidadVendida / totalBackers) * 100`
8. **Obtener progreso por dia**:
   - Llamar `_pedidoRepository.GetProgressByDayAsync(campaniaId, ct)` -> List<(DateTime, int, decimal)>
   - Post-procesar para calcular `Acumulado` iterando y sumando
9. **Construir CampaniaStatsDetailDto**
10. **Retornar ServiceResponse exitoso**
11. **Try-catch con logging**

**Calculo de ProyeccionFinal (formula):**
```csharp
// En DashboardService
public async Task<decimal?> CalcularProyeccionFinalAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
{
    var campania = await _campaniaRepository.GetByIdAsync(campaniaId, ct);
    if (campania?.FechaInicio == null || campania.FechaFin == null) return null;

    var velocidadDiaria = await GetVelocidadDiariaAsync(campaniaId, ct);
    var diasRestantes = Math.Max(0, (campania.FechaFin.Value - DateTime.UtcNow).Days);

    return Math.Round(campania.ImportePledgedActual + (velocidadDiaria * diasRestantes), 2);
}
```

**Acumulado en ProgressoPorDia:**
```csharp
var progressoPorDia = await _pedidoRepository.GetProgressByDayAsync(campaniaId, ct);

// Post-procesamiento para acumulado
decimal acumulado = 0;
var progressoDtos = progressoPorDia.Select(p =>
{
    acumulado += p.Total;
    return new ProgressoDiaDto
    {
        Fecha = p.Date.ToString("yyyy-MM-dd"),
        NumBackings = p.Count,
        TotalRecaudado = p.Total,
        Acumulado = acumulado
    };
}).ToList();
```

---

## 3. Validators

### 3.1 GetMisCampaniasQueryValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/GetMisCampaniasQueryValidator.cs`

**CRITICO:** Usar `ServiceResponseMessageType.X` constants (NO strings literales)

| Campo | Regla | Mensaje | ErrorCode (Constant) |
|-------|-------|---------|----------------------|
| UserId | NotEmpty | UserId es requerido | `ServiceResponseMessageType.Validation_Required` |
| Page | GreaterThanOrEqualTo(1) | Page debe ser mayor o igual a 1 | `ServiceResponseMessageType.Validation_InvalidRange` |
| PageSize | InclusiveBetween(1, 100) | PageSize debe estar entre 1 y 100 | `ServiceResponseMessageType.Validation_InvalidRange` |
| EstadoCampaniaId | InclusiveBetween(1, 5) (cuando no es null) | EstadoCampaniaId invalido | `ServiceResponseMessageType.Validation_InvalidRange` |

**Implementacion:**
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
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize debe estar entre 1 y 100")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.EstadoCampaniaId)
            .InclusiveBetween(1, 5)
            .When(x => x.EstadoCampaniaId.HasValue)
            .WithMessage("EstadoCampaniaId invalido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }
}
```

---

### 3.2 GetCampaniaBackingsQueryValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/GetCampaniaBackingsQueryValidator.cs`

| Campo | Regla | Mensaje | ErrorCode (Constant) |
|-------|-------|---------|----------------------|
| CampaniaId | NotEmpty | CampaniaId es requerido | `ServiceResponseMessageType.Validation_Required` |
| UserId | NotEmpty | UserId es requerido | `ServiceResponseMessageType.Validation_Required` |
| Page | GreaterThanOrEqualTo(1) | Page debe ser mayor o igual a 1 | `ServiceResponseMessageType.Validation_InvalidRange` |
| PageSize | InclusiveBetween(1, 100) | PageSize debe estar entre 1 y 100 | `ServiceResponseMessageType.Validation_InvalidRange` |

**Implementacion:**
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
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize debe estar entre 1 y 100")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }
}
```

---

### 3.3 GetCampaniaStatsQueryValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/GetCampaniaStatsQueryValidator.cs`

| Campo | Regla | Mensaje | ErrorCode (Constant) |
|-------|-------|---------|----------------------|
| CampaniaId | NotEmpty | CampaniaId es requerido | `ServiceResponseMessageType.Validation_Required` |
| UserId | NotEmpty | UserId es requerido | `ServiceResponseMessageType.Validation_Required` |

**Implementacion:**
```csharp
using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Validators;

public class GetCampaniaStatsQueryValidator : AbstractValidator<GetCampaniaStatsQuery>
{
    public GetCampaniaStatsQueryValidator()
    {
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("CampaniaId es requerido")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId es requerido")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
```

---

## 4. Archivos a Crear

```
Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
├── Dtos/
│   ├── DashboardResumenDto.cs
│   ├── MiCampaniaListItemDto.cs
│   ├── PaginatedResponse.cs (generico, reutilizable)
│   ├── CampaniaBackingListDto.cs
│   ├── CampaniaBackingStatsDto.cs
│   ├── UltimoBackingDto.cs
│   ├── CampaniaBackingItemDto.cs
│   ├── CampaniaStatsDetailDto.cs
│   ├── RewardStatDto.cs
│   └── ProgressoDiaDto.cs
│
├── Features/
│   ├── Dashboard/
│   │   └── Queries/
│   │       └── GetDashboardResumenQuery.cs  (Query + Handler en MISMO archivo)
│   │
│   └── Campanias/
│       ├── Queries/
│       │   ├── GetMisCampaniasQuery.cs      (Query + Handler)
│       │   ├── GetCampaniaBackingsQuery.cs  (Query + Handler)
│       │   └── GetCampaniaStatsQuery.cs     (Query + Handler)
│       │
│       └── Validators/
│           ├── GetMisCampaniasQueryValidator.cs
│           ├── GetCampaniaBackingsQueryValidator.cs
│           └── GetCampaniaStatsQueryValidator.cs
│
└── Interfaces/
    └── Services/
        └── IDashboardService.cs

Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/
├── Services/
│   └── DashboardService.cs
│
├── Repositories/
│   ├── CampaniaRepository.cs (AGREGAR metodos nuevos)
│   └── PedidoRepository.cs (AGREGAR metodos nuevos)
│
└── DependencyInjection.cs (AGREGAR registro de DashboardService)
```

**Total archivos NUEVOS:** 15 (10 DTOs + 4 Queries + 1 Service)

**Archivos MODIFICADOS:** 3 (2 Repositories + 1 DependencyInjection)

---

## 5. Patrones Importantes

### 5.1 Logica de Negocio en Handler

**Handler contiene:**
- Validacion de parametros (via FluentValidation)
- Validacion de autorizacion (artista ownership)
- Logica de negocio (calculos, transformaciones, reglas)
- Orquestacion de llamadas a Services
- Construccion de DTOs
- Manejo de errores (try-catch)

**Service contiene:**
- Calculos de metricas agregadas (sumas, promedios, proyecciones)
- Queries optimizados via Repository
- Cache (via IRequestCacheService)

**Repository contiene:**
- SOLO acceso a datos (queries EF Core)
- NO logica de negocio
- NO calculos

---

### 5.2 ServiceResponse SIEMPRE con Constants

**CRITICO:** NUNCA usar strings literales para ErrorCode.

```csharp
// CORRECTO - Usar constants
using WePlayRises.Crowdfunding.Domain.Constants;

return new ServiceResponse<T>
{
    Data = result,
    Messages = new List<ServiceResponseMessage>
    {
        new() { Message = "Operacion exitosa", ErrorCode = ServiceResponseMessageType.Success }
    }
};

// Error de validacion (automático desde FluentValidation)
return new ServiceResponse<T>
{
    Messages = validationResult.GetServiceResponseMessages()
};

// Error de negocio
return new ServiceResponse<T>
{
    Messages = new List<ServiceResponseMessage>
    {
        new() { Message = "Artista no encontrado", ErrorCode = UserAccess.Domain.Constants.ServiceResponseMessageType.NotFound_Artista }
    }
};

// INCORRECTO - Strings literales
new() { Message = "Error", ErrorCode = "NOT_FOUND" }  // NUNCA HACER ESTO
```

---

### 5.3 Validacion de Ownership (CRITICO)

**SIEMPRE validar ownership en queries de campania especifica:**

```csharp
// Patron estandar para GetCampaniaBackingsQuery y GetCampaniaStatsQuery

// 1. Obtener artista del usuario autenticado
var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
if (artista == null)
{
    return new ServiceResponse<T>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "Artista no encontrado", ErrorCode = UserAccess.Domain.Constants.ServiceResponseMessageType.NotFound_Artista }
        }
    };
}

// 2. Obtener campania
var campaniaId = new CampaniaCrowdfundingId(request.CampaniaId);
var campania = await _campaniaService.GetByIdAsync(campaniaId, ct);
if (campania == null)
{
    return new ServiceResponse<T>
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
    return new ServiceResponse<T>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "No tienes permiso para ver esta campania", ErrorCode = ServiceResponseMessageType.Auth_Forbidden }
        }
    };
}

// 4. Continuar con logica de negocio
```

---

### 5.4 Construccion Manual de DTOs (NO AutoMapper)

**Motivo:** Los DTOs de dashboard requieren calculos complejos (agregaciones, proyecciones SQL) que no se pueden hacer con AutoMapper.

**Patron recomendado:**
```csharp
// Construir DTO manualmente en Handler
var dto = new MiCampaniaListItemDto
{
    Id = campania.Id.Value,
    Titulo = campania.Titulo,
    // ... calculos directos
    PorcentajeProgreso = Math.Round((campania.ImportePledgedActual / campania.ImporteObjetivo) * 100, 2),
    // ... queries anidados
    NumBackers = await _pedidoRepository.CountByCampaniaIdAsync(campania.Id, ct)
};
```

**Alternativa (mejor rendimiento):** Usar proyecciones EF Core directas en Repository:
```csharp
// En Repository
var campanias = await _context.Campanias
    .AsNoTracking()
    .Where(c => c.ArtistaId == artistaId)
    .Select(c => new MiCampaniaListItemDto
    {
        Id = c.Id.Value,
        Titulo = c.Titulo,
        PorcentajeProgreso = c.ImporteObjetivo > 0
            ? Math.Round((c.ImportePledgedActual / c.ImporteObjetivo) * 100, 2)
            : 0,
        NumBackers = c.Pedidos.Count(p => p.EstadoPedidoId == 3),
        // ...
    })
    .ToListAsync(ct);
```

**Beneficio:** Query unico a DB, sin N+1 queries.

---

### 5.5 Anonimizacion de Backings

**CRITICO:** NO exponer nombre/email de backers anonimos.

```csharp
var backingDto = new CampaniaBackingItemDto
{
    Id = pedido.Id.Value,

    // Si PermitirMostrarNombre = false -> "Anonimo"
    NombreBacker = pedido.PermitirMostrarNombre
        ? await GetUserNameAsync(pedido.UserId, ct)  // Llamar a Identity
        : "Anonimo",

    // Si anonimo -> Email = null
    Email = pedido.PermitirMostrarNombre && !string.IsNullOrEmpty(pedido.UserId)
        ? await GetUserEmailAsync(pedido.UserId, ct)
        : null,

    // Flag para frontend
    EsAnonimo = !pedido.PermitirMostrarNombre
};
```

**Integracion con Identity:**
```csharp
// Inyectar UserManager en Handler
private readonly UserManager<ApplicationUser> _userManager;

private async Task<string> GetUserNameAsync(string? userId, CancellationToken ct)
{
    if (string.IsNullOrEmpty(userId)) return "Backer";

    var user = await _userManager.FindByIdAsync(userId);
    return user?.UserName ?? "Backer";
}

private async Task<string?> GetUserEmailAsync(string? userId, CancellationToken ct)
{
    if (string.IsNullOrEmpty(userId)) return null;

    var user = await _userManager.FindByIdAsync(userId);
    return user?.Email;
}
```

---

## 6. Checklist

- [ ] Query + Handler en MISMO archivo (REGLA CRITICA)
- [ ] Queries implementan `IRequest<ServiceResponse<T>>`
- [ ] **Constructores con `?? throw new ArgumentNullException` para TODAS las dependencias**
- [ ] Handlers inyectan Services, Validators, Logger (NUNCA DbContext)
- [ ] **Validators usan `ServiceResponseMessageType.X` constants (NO strings literales)**
- [ ] **Handlers usan `ServiceResponseMessageType.X` en respuestas (NO strings literales)**
- [ ] Try-catch con logging en TODOS los Handlers
- [ ] Validacion retorna ServiceResponse (NO throw exceptions)
- [ ] Validacion de ownership en queries de campania especifica
- [ ] Logica de anonimizacion para backings
- [ ] DTOs construidos manualmente (NO usar AutoMapper para queries complejas)
- [ ] Paginacion con limites (pageSize max 100)
- [ ] Queries optimizados con `AsNoTracking()` para lecturas
- [ ] Validators en carpeta `Validators/` separada

---

## 7. Dependencias Requeridas

### 7.1 Services Nuevos

**IDashboardService** (NUEVO)
```csharp
Task<(decimal, int, int, int)> GetResumenAsync(ArtistaId, ct);
Task<decimal> GetBackingPromedioAsync(CampaniaCrowdfundingId, ct);
Task<string?> GetRewardMasPopularAsync(CampaniaCrowdfundingId, ct);
Task<decimal> GetVelocidadDiariaAsync(CampaniaCrowdfundingId, ct);
Task<decimal?> CalcularProyeccionFinalAsync(CampaniaCrowdfundingId, ct);
```

### 7.2 Repository Extensions

**ICampaniaRepository** (AGREGAR metodos)
```csharp
Task<IReadOnlyList<CampaniaCrowdfunding>> GetMisCampaniasPaginatedAsync(ArtistaId, estadoId?, page, pageSize, ct);
Task<int> CountMisCampaniasAsync(ArtistaId, estadoId?, ct);
```

**IPedidoRepository** (AGREGAR metodos)
```csharp
Task<IReadOnlyList<PedidoCrowdfunding>> GetByCampaniaIdPaginatedAsync(CampaniaCrowdfundingId, page, pageSize, ct);
Task<int> CountByCampaniaIdAsync(CampaniaCrowdfundingId, ct);
Task<Dictionary<Guid, int>> GetRewardStatsByCampaniaIdAsync(CampaniaCrowdfundingId, ct);
Task<PedidoCrowdfunding?> GetLastByCampaniaIdAsync(CampaniaCrowdfundingId, ct);
Task<IReadOnlyList<(DateTime Date, int Count, decimal Total)>> GetProgressByDayAsync(CampaniaCrowdfundingId, ct);
```

### 7.3 Services Existentes (Reutilizar)

- `IArtistaService.GetByUserIdAsync()` - Validar artista (modulo UserAccess)
- `ICampaniaService.GetByIdAsync()` - Obtener campania
- `IRewardRepository.GetByIdAsync()` - Obtener nombre de reward

### 7.4 Identity (ASP.NET Core)

- `UserManager<ApplicationUser>.FindByIdAsync()` - Obtener nombre/email de backers

---

## 8. Notas de Implementacion

### 8.1 NO se requieren Migraciones

Esta feature NO modifica entidades existentes. Solo lee datos.

**Opcional:** Agregar indices para mejorar rendimiento de queries:
```sql
CREATE INDEX IX_CampaniaCrowdfunding_ArtistaId ON CampaniaCrowdfunding(ArtistaId);
CREATE INDEX IX_CampaniaCrowdfunding_ArtistaId_EstadoCampaniaId ON CampaniaCrowdfunding(ArtistaId, EstadoCampaniaId);
CREATE INDEX IX_PedidoCrowdfunding_CampaniaId_EstadoPedidoId ON PedidoCrowdfunding(CampaniaId, EstadoPedidoId);
CREATE INDEX IX_PedidoCrowdfunding_FechaCreacion ON PedidoCrowdfunding(FechaCreacion DESC);
```

### 8.2 NO se requieren nuevas Constantes

Las constantes existentes en `ServiceResponseMessageType.cs` son suficientes:
- `Success` (0000)
- `Validation_Required` (1001)
- `Validation_InvalidRange` (1007)
- `NotFound_Artista` (2002 - UserAccess.Domain.Constants)
- `NotFound_Campania` (2003)
- `Auth_Forbidden` (3002)
- `Internal_UnexpectedError` (5000)

---

## 9. Siguiente Paso

**Orden de implementacion recomendado:**

1. **Crear DTOs** (10 archivos)
2. **Crear IDashboardService + DashboardService** (calculos de metricas)
3. **Extender Repositories** (agregar metodos nuevos)
4. **Registrar DI** (DashboardService)
5. **Implementar Queries + Handlers** (4 queries)
6. **Crear Validators** (3 validators)
7. **Tests unitarios** (Handlers + DashboardService)
8. **Tests de integracion** (endpoints)

**Estimacion:** ~9 horas (segun plan hexagonal-architecture.md)

---

**Fin del Plan CQRS: Dashboard de Artista**
