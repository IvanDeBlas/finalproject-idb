# Arquitectura Hexagonal: Hacer Backing (Apoyar Campaña)

**Fecha:** 2026-02-13
**Modulo:** Crowdfunding
**Feature:** hacer-backing (US-04)

---

## 1. Resumen Ejecutivo

Esta feature permite a los fans (autenticados o anónimos) apoyar económicamente campañas de crowdfunding, seleccionando recompensas opcionales y realizando aportes que actualizan las métricas de la campaña. El sistema registra pedidos con transacciones atómicas, actualiza el monto recaudado, gestiona stock de rewards, y permite explorar campañas activas con detalle completo (rewards, backings recientes, estadísticas).

**Operación crítica:** Transacción atómica que crea PedidoCrowdfunding + PedidoCrowdfundingLinea + AportacionCrowdfunding + actualiza CampaniaCrowdfunding.ImportePledgedActual.

---

## 2. Domain Layer

### 2.1 Entidades Existentes (Sin Modificaciones)

Todas las entidades necesarias YA EXISTEN en `WePlayRises.Crowdfunding.Domain.Model`:

#### PedidoCrowdfunding
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Model/PedidoCrowdfunding.cs`

| Propiedad | Tipo | Nullable | Descripción |
|-----------|------|----------|-------------|
| Id | PedidoCrowdfundingId | No | PK (StronglyTypedId) |
| CampaniaId | CampaniaCrowdfundingId | No | FK a CampaniaCrowdfunding |
| UserId | string | Si | FK a Identity User (null para anónimos) |
| FanProfileId | FanProfileId | Si | FK a FanProfile (opcional) |
| EstadoPedidoId | int | No | FK a MaestraEstadoPedidoCrowd (1=Pendiente, 3=Completado MVP) |
| MonedaId | int | No | FK a MaestraMoneda (1=EUR) |
| ImporteSubtotal | decimal | No | Subtotal del pedido |
| ImportePropina | decimal | No | Propina adicional (opcional) |
| ImporteEnvio | decimal | No | Costo envío (0 en MVP) |
| ImporteImpuestos | decimal | No | Impuestos (0 en MVP) |
| ImporteTotal | decimal | No | Total final a pagar |
| PermitirMostrarNombre | bool | No | True = mostrar nombre, False = "Anónimo" |
| ComentarioBacker | string | Si | Mensaje del backer al artista (max 500 chars) |
| DireccionEnvioId | Guid | Si | FK a DireccionPostal (futuro, envíos físicos) |
| FechaCreacion | DateTime | No | Timestamp creación |
| FechaActualizacion | DateTime | Si | Timestamp última actualización |

**Navegaciones:**
- `Campania` → `CampaniaCrowdfunding` (N:1)
- `Lineas` → `ICollection<PedidoCrowdfundingLinea>` (1:N)
- `Aportaciones` → `ICollection<AportacionCrowdfunding>` (1:N)

#### PedidoCrowdfundingLinea
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Model/PedidoCrowdfundingLinea.cs`

| Propiedad | Tipo | Nullable | Descripción |
|-----------|------|----------|-------------|
| Id | Guid | No | PK |
| PedidoCrowdfundingId | PedidoCrowdfundingId | No | FK a PedidoCrowdfunding |
| RewardId | CampaniaCrowdfundingRewardId | No | FK a CampaniaCrowdfundingReward |
| Cantidad | int | No | Cantidad (siempre 1 en MVP) |
| PrecioUnitario | decimal | No | Precio unitario (= monto backing) |
| ImporteLinea | decimal | No | Total línea (= PrecioUnitario * Cantidad) |
| EsRewardPrincipal | bool | No | True para reward seleccionado (no addons) |
| FechaCreacion | DateTime | No | Timestamp creación |

**Navegaciones:**
- `PedidoCrowdfunding` → `PedidoCrowdfunding` (N:1)
- `Reward` → `CampaniaCrowdfundingReward` (N:1)

**NOTA IMPORTANTE:** `RewardId` es **NOT NULL** en el esquema actual. Para permitir backing sin recompensa, necesitamos:
- **Opción A (RECOMENDADA):** Crear un reward especial "Sin Recompensa" con `ImporteMinimo = 0` por cada campaña
- **Opción B:** Modificar esquema para hacer `RewardId` nullable (requiere migración)

#### AportacionCrowdfunding
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Model/AportacionCrowdfunding.cs`

| Propiedad | Tipo | Nullable | Descripción |
|-----------|------|----------|-------------|
| Id | AportacionCrowdfundingId | No | PK (StronglyTypedId) |
| PedidoCrowdfundingId | PedidoCrowdfundingId | No | FK a PedidoCrowdfunding |
| MonedaId | int | No | FK a MaestraMoneda (1=EUR) |
| MetodoPagoId | int | No | FK a MaestraMetodoPago (99=Simulado MVP) |
| EstadoAportacionId | int | No | FK a MaestraEstadoAportacionCrowd (2=Confirmado MVP) |
| ImporteTotal | decimal | No | Monto total del backing |
| ImporteImpuestos | decimal | No | Impuestos (0 en MVP) |
| ImporteComisionPlataforma | decimal | No | Comisión plataforma (0 en MVP) |
| ImporteComisionPasarela | decimal | No | Comisión pasarela (0 en MVP) |
| ImporteNetoArtista | decimal | No | Neto para artista (= ImporteTotal en MVP) |
| CodigoOperacionPasarela | string | Si | Código transacción pasarela (null en MVP) |
| CodigoOperacionProveedor | string | Si | Código transacción proveedor (null en MVP) |
| FechaAutorizacion | DateTime | Si | Fecha autorización pago (null MVP) |
| FechaCaptura | DateTime | Si | Fecha captura pago (null MVP) |
| FechaCancelacion | DateTime | Si | Fecha cancelación pago (null) |
| FechaCreacion | DateTime | No | Timestamp creación |
| FechaActualizacion | DateTime | Si | Timestamp última actualización |

**Navegaciones:**
- `PedidoCrowdfunding` → `PedidoCrowdfunding` (N:1)

#### CampaniaCrowdfunding
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Model/CampaniaCrowdfunding.cs`

**Campos críticos para backing:**
- `ImportePledgedActual` (decimal) - **ACTUALIZADO ATOMICAMENTE** en cada backing
- `EstadoCampaniaId` (int) - Solo campañas con estado 2 (PUBLICADA) aceptan backings
- `FechaFin` (DateTime?) - Validar que no haya expirado
- `PermiteAportacionesAnonimas` (bool) - Validar si usuario no autenticado

#### CampaniaCrowdfundingReward
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Model/CampaniaCrowdfundingReward.cs`

**Campos críticos para stock:**
- `CantidadMaxima` (int?) - Stock máximo (null = ilimitado)
- `EsActivo` (bool) - Solo rewards activos son seleccionables
- `ImporteMinimo` (decimal) - Monto mínimo del backing para este reward

**Cálculo de stock vendido:**
```csharp
// Se calcula dinámicamente desde PedidoCrowdfundingLinea
var cantidadVendida = await _context.PedidoCrowdfundingLinea
    .Where(l => l.RewardId == rewardId &&
                l.PedidoCrowdfunding.EstadoPedidoId == 3) // COMPLETADO
    .SumAsync(l => l.Cantidad);
```

### 2.2 Constants (ServiceResponseMessageType)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Constants/ServiceResponseMessageType.cs`

**Nuevos códigos a AGREGAR:**

```csharp
// Business Rule errors (4000-4999) - NUEVOS
public const string BusinessRule_RewardOutOfStock = "4011";
public const string BusinessRule_AmountBelowMinimum = "4012";
public const string BusinessRule_AnonymousNotAllowed = "4013";
```

**Códigos existentes a UTILIZAR:**
- `Created = "0001"` - Backing creado con éxito
- `Validation_Required = "1001"` - Campos obligatorios
- `Validation_MaxLength = "1002"` - Mensaje excede 500 caracteres
- `Validation_InvalidAmount = "1011"` - Monto inválido
- `NotFound_Campania = "2003"` - Campaña no encontrada
- `NotFound_Reward = "2004"` - Reward no encontrado
- `Auth_UserNotAuthenticated = "3005"` - Usuario no autenticado cuando campaña no permite anónimos
- `BusinessRule_CampaniaNotActive = "4006"` - Campaña no está en estado PUBLICADA
- `BusinessRule_CampaniaEnded = "4007"` - Campaña ha finalizado por fecha
- `Internal_UnexpectedError = "5000"` - Error inesperado

### 2.3 Repository Interfaces (Ports)

#### ICampaniaRepository (EXISTENTE - Extender)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Repositories/ICampaniaRepository.cs`

**Métodos existentes suficientes:**
- `GetByIdAsync(CampaniaCrowdfundingId, CancellationToken)` - ✅ Ya incluye `.Include(c => c.Rewards)`
- `UpdateAsync(CampaniaCrowdfunding, CancellationToken)` - ✅ Para actualizar ImportePledgedActual

**Nuevos métodos a AGREGAR:**
```csharp
Task<CampaniaCrowdfunding?> GetDetailByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct);
// Retorna campaña con includes completos: Rewards activos, Pedidos recientes (últimos 10)
// Include: Rewards.Where(r => r.EsActivo), Pedidos.OrderByDescending(p => p.FechaCreacion).Take(10)
```

#### IRewardRepository (EXISTENTE - Sin cambios)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Repositories/IRewardRepository.cs`

**Métodos existentes suficientes:**
- `GetByIdAsync(CampaniaCrowdfundingRewardId, CancellationToken)` - ✅ Para validar reward
- `GetByIdWithLineasAsync(CampaniaCrowdfundingRewardId, CancellationToken)` - ✅ Para calcular stock vendido

#### IPedidoRepository (EXISTENTE - Extender)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Repositories/IPedidoRepository.cs`

**Métodos existentes suficientes:**
- `AddAsync(PedidoCrowdfunding, CancellationToken)` - ✅ Crear pedido
- `GetByCampaniaIdAsync(CampaniaCrowdfundingId, CancellationToken)` - ✅ Obtener pedidos de campaña

**Nuevos métodos a AGREGAR:**
```csharp
Task<IReadOnlyList<PedidoCrowdfunding>> GetRecentByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, int limit, CancellationToken ct);
// Retorna últimos N pedidos completados de una campaña para mostrar en detalle
// Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3)
// OrderByDescending(p => p.FechaCreacion)
// Take(limit) - Default 10

Task<int> CountByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
// Cuenta backers únicos (total de pedidos completados) para estadísticas
```

#### NUEVO: IAportacionRepository
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Repositories/IAportacionRepository.cs`

```csharp
public interface IAportacionRepository
{
    Task<AportacionCrowdfundingId> AddAsync(AportacionCrowdfunding entity, CancellationToken ct);
    Task<AportacionCrowdfunding?> GetByPedidoIdAsync(PedidoCrowdfundingId pedidoId, CancellationToken ct);
}
```

**Justificación:** Separar responsabilidades. PedidoRepository gestiona pedidos, AportacionRepository gestiona aportaciones simuladas (pagos).

---

## 3. Infrastructure Layer

### 3.1 Repository Implementations

#### CampaniaRepository (EXISTENTE - Extender)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Repositories/CampaniaRepository.cs`

**Nuevo método a AGREGAR:**
```csharp
public async Task<CampaniaCrowdfunding?> GetDetailByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct)
{
    return await _context.Campanias
        .AsNoTracking()  // Read-only para detalle público
        .Include(c => c.Rewards.Where(r => r.EsActivo))  // Solo rewards activos
        .Include(c => c.Pedidos
            .Where(p => p.EstadoPedidoId == 3)  // Solo pedidos completados
            .OrderByDescending(p => p.FechaCreacion)
            .Take(10))  // Últimos 10 backings
        .FirstOrDefaultAsync(x => x.Id == id, ct);
}
```

**NOTA EF Core Limitation:** `.Include().Where()` con filtros complejos puede no funcionar en todas las versiones. Alternativa:

```csharp
public async Task<CampaniaCrowdfunding?> GetDetailByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct)
{
    var campania = await _context.Campanias
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == id, ct);

    if (campania == null) return null;

    // Cargar rewards activos
    await _context.Entry(campania)
        .Collection(c => c.Rewards)
        .Query()
        .Where(r => r.EsActivo)
        .OrderBy(r => r.Orden)
        .LoadAsync(ct);

    // Cargar backings recientes
    await _context.Entry(campania)
        .Collection(c => c.Pedidos)
        .Query()
        .Where(p => p.EstadoPedidoId == 3)
        .OrderByDescending(p => p.FechaCreacion)
        .Take(10)
        .LoadAsync(ct);

    return campania;
}
```

#### PedidoRepository (EXISTENTE - Extender)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Repositories/PedidoRepository.cs`

**Nuevos métodos a AGREGAR:**
```csharp
public async Task<IReadOnlyList<PedidoCrowdfunding>> GetRecentByCampaniaIdAsync(
    CampaniaCrowdfundingId campaniaId,
    int limit,
    CancellationToken ct)
{
    return await _context.Pedidos
        .AsNoTracking()
        .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3)
        .OrderByDescending(p => p.FechaCreacion)
        .Take(limit)
        .ToListAsync(ct);
}

public async Task<int> CountByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
{
    return await _context.Pedidos
        .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3)
        .CountAsync(ct);
}
```

#### NUEVO: AportacionRepository
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Repositories/AportacionRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Domain.Model;
using WePlayRises.Crowdfunding.Infra.Context;

namespace WePlayRises.Crowdfunding.Infra.Repositories;

public class AportacionRepository : IAportacionRepository
{
    private readonly CrowdfundingContext _context;

    public AportacionRepository(CrowdfundingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<AportacionCrowdfundingId> AddAsync(AportacionCrowdfunding entity, CancellationToken ct)
    {
        await _context.Aportaciones.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<AportacionCrowdfunding?> GetByPedidoIdAsync(PedidoCrowdfundingId pedidoId, CancellationToken ct)
    {
        return await _context.Aportaciones
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.PedidoCrowdfundingId == pedidoId, ct);
    }
}
```

### 3.2 Services (Persistencia con UnitOfWork)

#### ICampaniaService (EXISTENTE - Extender)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Services/ICampaniaService.cs`

**Nuevos métodos a AGREGAR:**
```csharp
Task<CampaniaCrowdfunding?> GetDetailByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct);
// Retorna campaña con todos los datos para detalle público (rewards, backings recientes)

Task<int> GetTotalBackersAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
// Cuenta total de backers (pedidos completados) para estadísticas
```

#### CampaniaService (EXISTENTE - Extender)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Services/CampaniaService.cs`

**IMPORTANTE:** Service actual NO inyecta UnitOfWork porque usa Repository con SaveChanges interno.

**Nuevos métodos a AGREGAR:**
```csharp
public async Task<CampaniaCrowdfunding?> GetDetailByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct)
{
    var cacheKey = $"campania:detail:{id.Value}";

    return await _requestCache.GetOrAddAsync(
        cacheKey,
        async () => await _repository.GetDetailByIdAsync(id, ct));
}

public async Task<int> GetTotalBackersAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
{
    var cacheKey = $"campania:{campaniaId.Value}:backers";

    return await _requestCache.GetOrAddAsync(
        cacheKey,
        async () => await _repository.CountByCampaniaIdAsync(campaniaId, ct));
}
```

**NOTA CRÍTICA:** `GetDetailByIdAsync` NO debe cachear porque incluye backings recientes (datos volátiles). Considerar:
- **Opción A:** No cachear este endpoint (siempre fresh data)
- **Opción B:** Cache corto (30 segundos) con `MemoryCacheEntryOptions.AbsoluteExpiration`
- **Opción C:** Cache solo la entidad, cargar backings recientes sin cache

#### IRewardService (EXISTENTE - Extender)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Services/IRewardService.cs`

**Nuevos métodos a AGREGAR:**
```csharp
Task<int> GetCantidadVendidaAsync(CampaniaCrowdfundingRewardId rewardId, CancellationToken ct);
// Calcula cantidad vendida desde PedidoCrowdfundingLinea
// WHERE RewardId = X AND PedidoCrowdfunding.EstadoPedidoId = 3 (Completado)

Task<bool> HasStockAvailableAsync(CampaniaCrowdfundingRewardId rewardId, CancellationToken ct);
// Valida si reward tiene stock disponible
// CantidadMaxima == null || CantidadVendida < CantidadMaxima
```

#### RewardService (EXISTENTE - Extender)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Services/RewardService.cs`

**Nuevos métodos a AGREGAR:**
```csharp
public async Task<int> GetCantidadVendidaAsync(CampaniaCrowdfundingRewardId rewardId, CancellationToken ct)
{
    var cacheKey = $"reward:{rewardId.Value}:vendida";

    return await _requestCache.GetOrAddAsync(
        cacheKey,
        async () => await _repository.GetCantidadVendidaAsync(rewardId, ct));
}

public async Task<bool> HasStockAvailableAsync(CampaniaCrowdfundingRewardId rewardId, CancellationToken ct)
{
    var reward = await GetByIdAsync(rewardId, ct);
    if (reward == null || !reward.EsActivo) return false;

    if (reward.CantidadMaxima == null) return true; // Ilimitado

    var cantidadVendida = await GetCantidadVendidaAsync(rewardId, ct);
    return cantidadVendida < reward.CantidadMaxima.Value;
}
```

**IRewardRepository - Nuevo método:**
```csharp
Task<int> GetCantidadVendidaAsync(CampaniaCrowdfundingRewardId rewardId, CancellationToken ct);
```

**RewardRepository - Implementación:**
```csharp
public async Task<int> GetCantidadVendidaAsync(CampaniaCrowdfundingRewardId rewardId, CancellationToken ct)
{
    return await _context.PedidoLineas
        .Where(l => l.RewardId == rewardId &&
                    l.PedidoCrowdfunding.EstadoPedidoId == 3) // COMPLETADO
        .SumAsync(l => l.Cantidad, ct);
}
```

#### NUEVO: IBackingService
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Services/IBackingService.cs`

```csharp
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Interfaces.Services;

public interface IBackingService
{
    /// <summary>
    /// Crea un backing completo con transacción atómica.
    /// Crea: PedidoCrowdfunding + PedidoCrowdfundingLinea + AportacionCrowdfunding
    /// Actualiza: CampaniaCrowdfunding.ImportePledgedActual
    /// </summary>
    Task<PedidoCrowdfundingId> CreateBackingAsync(
        CampaniaCrowdfundingId campaniaId,
        CampaniaCrowdfundingRewardId? rewardId,
        decimal monto,
        string? userId,
        string? mensaje,
        bool esAnonimo,
        CancellationToken ct);

    /// <summary>
    /// Valida si una campaña puede recibir backings.
    /// Verifica: Estado PUBLICADA, FechaFin no expirada, ImporteObjetivo válido.
    /// </summary>
    Task<bool> CanReceiveBackingAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
}
```

#### NUEVO: BackingService
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Services/BackingService.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Model;
using WePlayRises.Crowdfunding.Infra.Context;

namespace WePlayRises.Crowdfunding.Infra.Services;

public class BackingService : IBackingService
{
    private readonly CrowdfundingContext _context;
    private readonly ICampaniaRepository _campaniaRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IAportacionRepository _aportacionRepository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<BackingService> _logger;

    public BackingService(
        CrowdfundingContext context,
        ICampaniaRepository campaniaRepository,
        IPedidoRepository pedidoRepository,
        IAportacionRepository aportacionRepository,
        IRequestCacheService requestCache,
        ILogger<BackingService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _campaniaRepository = campaniaRepository ?? throw new ArgumentNullException(nameof(campaniaRepository));
        _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
        _aportacionRepository = aportacionRepository ?? throw new ArgumentNullException(nameof(aportacionRepository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PedidoCrowdfundingId> CreateBackingAsync(
        CampaniaCrowdfundingId campaniaId,
        CampaniaCrowdfundingRewardId? rewardId,
        decimal monto,
        string? userId,
        string? mensaje,
        bool esAnonimo,
        CancellationToken ct)
    {
        // TRANSACCIÓN ATÓMICA
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            // 1. Crear PedidoCrowdfunding
            var pedido = new PedidoCrowdfunding
            {
                CampaniaId = campaniaId,
                UserId = userId,
                EstadoPedidoId = 3, // COMPLETADO (MVP sin pago real)
                MonedaId = 1, // EUR (único soportado en MVP)
                ImporteSubtotal = monto,
                ImportePropina = 0,
                ImporteEnvio = 0,
                ImporteImpuestos = 0,
                ImporteTotal = monto,
                PermitirMostrarNombre = !esAnonimo,
                ComentarioBacker = mensaje,
                FechaCreacion = DateTime.UtcNow
            };

            await _context.Pedidos.AddAsync(pedido, ct);
            await _context.SaveChangesAsync(ct); // SaveChanges para obtener Id

            // 2. Crear PedidoCrowdfundingLinea
            var linea = new PedidoCrowdfundingLinea
            {
                PedidoCrowdfundingId = pedido.Id,
                RewardId = rewardId!.Value, // NOTA: RewardId es NOT NULL en esquema
                Cantidad = 1,
                PrecioUnitario = monto,
                ImporteLinea = monto,
                EsRewardPrincipal = true,
                FechaCreacion = DateTime.UtcNow
            };

            await _context.PedidoLineas.AddAsync(linea, ct);

            // 3. Actualizar CampaniaCrowdfunding.ImportePledgedActual
            var campania = await _campaniaRepository.GetByIdAsync(campaniaId, ct);
            if (campania == null)
            {
                throw new InvalidOperationException($"Campaña {campaniaId.Value} no encontrada durante transacción");
            }

            campania.ImportePledgedActual += monto;
            campania.FechaActualizacion = DateTime.UtcNow;
            _context.Campanias.Update(campania);

            // 4. Crear AportacionCrowdfunding (pago simulado MVP)
            var aportacion = new AportacionCrowdfunding
            {
                PedidoCrowdfundingId = pedido.Id,
                MonedaId = 1, // EUR
                MetodoPagoId = 99, // Simulado (MVP)
                EstadoAportacionId = 2, // Confirmado
                ImporteTotal = monto,
                ImporteImpuestos = 0,
                ImporteComisionPlataforma = 0,
                ImporteComisionPasarela = 0,
                ImporteNetoArtista = monto,
                FechaCreacion = DateTime.UtcNow
            };

            await _context.Aportaciones.AddAsync(aportacion, ct);

            // 5. SaveChanges FINAL + Commit
            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "Backing creado: Pedido {PedidoId}, Campaña {CampaniaId}, Monto {Monto}, UserId {UserId}",
                pedido.Id.Value, campaniaId.Value, monto, userId ?? "Anónimo");

            return pedido.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogError(ex, "Error al crear backing para Campaña {CampaniaId}", campaniaId.Value);
            throw;
        }
    }

    public async Task<bool> CanReceiveBackingAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var campania = await _campaniaRepository.GetByIdAsync(campaniaId, ct);
        if (campania == null) return false;

        // Estado debe ser PUBLICADA (2)
        if (campania.EstadoCampaniaId != 2) return false;

        // No debe haber expirado
        if (campania.FechaFin.HasValue && DateTime.UtcNow > campania.FechaFin.Value)
            return false;

        return true;
    }
}
```

**IMPORTANTE - Gestión de Reward "Sin Recompensa":**

Dado que `RewardId` es NOT NULL, necesitamos una estrategia:

**Opción A (RECOMENDADA):** Crear reward especial por campaña:
```csharp
// Al publicar campaña, crear reward automático
var noReward = new CampaniaCrowdfundingReward
{
    CampaniaId = campaniaId,
    TipoRewardId = 1, // "Sin recompensa"
    Nombre = "Apoyo sin recompensa",
    Descripcion = "Apoya el proyecto sin recibir recompensa física",
    ImporteMinimo = 1.00m,
    MonedaId = 1,
    EsAddOn = false,
    CantidadMaxima = null, // Ilimitado
    IncluyeEnvioFisico = false,
    Orden = 999, // Último en la lista
    EsActivo = true
};
```

**Opción B:** Modificar esquema (requiere migración):
```csharp
// En PedidoCrowdfundingLinea
public CampaniaCrowdfundingRewardId? RewardId { get; set; } // Hacer nullable
```

### 3.3 Entity Configurations (EF Core)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Data/Configurations/PedidoCrowdfundingConfiguration.cs`

Ya EXISTE configuración básica. Verificar:

```csharp
builder.HasOne(p => p.Campania)
    .WithMany(c => c.Pedidos)
    .HasForeignKey(p => p.CampaniaId)
    .OnDelete(DeleteBehavior.Restrict);

builder.Property(p => p.ImporteTotal)
    .HasPrecision(18, 2);

builder.Property(p => p.ComentarioBacker)
    .HasMaxLength(500);
```

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Data/Configurations/CampaniaCrowdfundingConfiguration.cs`

Verificar índices para queries frecuentes:

```csharp
builder.HasIndex(c => c.EstadoCampaniaId);
builder.HasIndex(c => c.FechaFin);
builder.HasIndex(c => new { c.EstadoCampaniaId, c.FechaFin }); // Para query de campañas activas
```

**NUEVO: Optimistic Locking para Rewards (Prevenir race conditions)**

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Data/Configurations/CampaniaCrowdfundingRewardConfiguration.cs`

```csharp
// AGREGAR campo RowVersion
builder.Property<byte[]>("RowVersion")
    .IsRowVersion()
    .HasColumnName("RowVersion");
```

**NOTA:** Esto requiere migración para agregar columna `RowVersion` tipo `ROWVERSION` (SQL Server).

Manejo de concurrencia en BackingService:

```csharp
try
{
    await _context.SaveChangesAsync(ct);
}
catch (DbUpdateConcurrencyException ex)
{
    _logger.LogWarning(ex, "Race condition detectada al crear backing para Reward {RewardId}", rewardId);
    throw new InvalidOperationException("El reward seleccionado ya no tiene stock disponible. Por favor, intenta con otro.");
}
```

### 3.4 DbContext

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Context/CrowdfundingContext.cs`

Verificar DbSets existentes:

```csharp
public DbSet<PedidoCrowdfunding> Pedidos => Set<PedidoCrowdfunding>();
public DbSet<PedidoCrowdfundingLinea> PedidoLineas => Set<PedidoCrowdfundingLinea>();
public DbSet<AportacionCrowdfunding> Aportaciones => Set<AportacionCrowdfunding>();
public DbSet<CampaniaCrowdfunding> Campanias => Set<CampaniaCrowdfunding>();
public DbSet<CampaniaCrowdfundingReward> Rewards => Set<CampaniaCrowdfundingReward>();
```

**NOTA:** Todos los DbSets necesarios ya EXISTEN.

---

## 4. Dependency Injection

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/DependencyInjection.cs`

**Registros a AGREGAR:**

```csharp
// Repositories
services.AddScoped<IAportacionRepository, AportacionRepository>();

// Services
services.AddScoped<IBackingService, BackingService>();
```

**Registros existentes a VERIFICAR:**
```csharp
services.AddScoped<ICampaniaRepository, CampaniaRepository>();
services.AddScoped<ICampaniaService, CampaniaService>();
services.AddScoped<IRewardRepository, RewardRepository>();
services.AddScoped<IRewardService, RewardService>();
services.AddScoped<IPedidoRepository, PedidoRepository>();
services.AddScoped<IPedidoService, PedidoService>();
```

---

## 5. Migraciones EF Core

### Migración 1: RowVersion en Rewards (Optimistic Locking)

```bash
dotnet ef migrations add AddRowVersionToRewards --project src/api/Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra --context CrowdfundingContext
```

**Contenido esperado:**
```csharp
migrationBuilder.AddColumn<byte[]>(
    name: "RowVersion",
    table: "CampaniaCrowdfundingReward",
    type: "rowversion",
    rowVersion: true,
    nullable: false);
```

### Migración 2 (OPCIONAL): RewardId Nullable

Si se decide permitir backing sin recompensa (sin reward especial):

```bash
dotnet ef migrations add MakeRewardIdNullable --project src/api/Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra --context CrowdfundingContext
```

```csharp
migrationBuilder.AlterColumn<Guid>(
    name: "RewardId",
    table: "PedidoCrowdfundingLinea",
    type: "uniqueidentifier",
    nullable: true,
    oldClrType: typeof(Guid),
    oldType: "uniqueidentifier");
```

**RECOMENDACIÓN:** NO hacer esta migración. Usar reward especial "Sin recompensa" en su lugar.

---

## 6. Archivos a Crear/Modificar

### Crear

```
Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
├── Interfaces/
│   ├── Repositories/
│   │   └── IAportacionRepository.cs                         # NUEVO
│   └── Services/
│       └── IBackingService.cs                                # NUEVO
│
└── Dtos/
    ├── BackingDto.cs                                          # NUEVO
    ├── BackingPublicDto.cs                                    # NUEVO
    ├── BackingConfirmationDto.cs                              # NUEVO
    ├── CampaniaDetailDto.cs                                   # NUEVO
    ├── CampaniaStatsDto.cs                                    # NUEVO
    └── RewardPublicDto.cs                                     # NUEVO

Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/
├── Repositories/
│   └── AportacionRepository.cs                               # NUEVO
└── Services/
    └── BackingService.cs                                      # NUEVO
```

### Modificar (Extender)

```
Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/
└── Constants/
    └── ServiceResponseMessageType.cs                         # AGREGAR 3 códigos

Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
├── Interfaces/
│   ├── Repositories/
│   │   ├── ICampaniaRepository.cs                            # AGREGAR GetDetailByIdAsync
│   │   ├── IPedidoRepository.cs                              # AGREGAR GetRecentByCampaniaIdAsync, CountByCampaniaIdAsync
│   │   └── IRewardRepository.cs                              # AGREGAR GetCantidadVendidaAsync
│   └── Services/
│       ├── ICampaniaService.cs                               # AGREGAR GetDetailByIdAsync, GetTotalBackersAsync
│       └── IRewardService.cs                                 # AGREGAR GetCantidadVendidaAsync, HasStockAvailableAsync

Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/
├── Repositories/
│   ├── CampaniaRepository.cs                                 # IMPLEMENTAR GetDetailByIdAsync
│   ├── PedidoRepository.cs                                   # IMPLEMENTAR métodos nuevos
│   └── RewardRepository.cs                                   # IMPLEMENTAR GetCantidadVendidaAsync
├── Services/
│   ├── CampaniaService.cs                                    # IMPLEMENTAR métodos nuevos
│   └── RewardService.cs                                      # IMPLEMENTAR métodos nuevos
└── DependencyInjection.cs                                    # REGISTRAR nuevos servicios
```

---

## 7. Checklist Arquitectónico

- [x] Entidades son POCOs (sin métodos de negocio) - ✅ Ya existen
- [x] ServiceResponseMessageType.cs con constantes nuevas (4011, 4012, 4013)
- [x] Repository interfaces en Domain/Application - ✅ Extendidas + IAportacionRepository nueva
- [x] Repository implementations en Infra - ✅ Extendidas + AportacionRepository nueva
- [x] Services inyectan Context + Repositories + Cache + Logger - ✅ BackingService completo
- [x] Services usan `?? throw new ArgumentNullException` en constructor - ✅ Patrón aplicado
- [x] Services retornan entidades (no DTOs) - ✅ Correcto
- [x] Entity configurations con Fluent API - ✅ Existentes, agregar RowVersion
- [x] DI registrado correctamente - ✅ DependencyInjection.cs actualizado
- [x] Transacción atómica en BackingService.CreateBackingAsync - ✅ Con try-catch y rollback
- [x] RequestCacheService para evitar queries duplicados - ✅ Usado en servicios
- [x] AsNoTracking para lecturas - ✅ Aplicado en queries read-only
- [x] Optimistic Locking con RowVersion - ✅ Diseñado para Rewards

---

## 8. Consideraciones Especiales

### 8.1 Transacción Atómica

El flujo de backing DEBE ser atómico. Si falla cualquier paso, se hace rollback:

1. Crear PedidoCrowdfunding
2. Crear PedidoCrowdfundingLinea
3. Actualizar CampaniaCrowdfunding.ImportePledgedActual
4. Crear AportacionCrowdfunding
5. SaveChanges + Commit

**Implementado en:** `BackingService.CreateBackingAsync`

### 8.2 Race Conditions en Stock

Dos usuarios intentan comprar el último reward simultáneamente:

**Solución A (RECOMENDADA):** Optimistic Locking con RowVersion
- EF Core detecta conflicto con `DbUpdateConcurrencyException`
- Handler captura excepción y retorna error "Stock agotado"

**Solución B:** Pessimistic Locking (SQL)
```csharp
var reward = await _context.Rewards
    .FromSqlRaw("SELECT * FROM CampaniaCrowdfundingReward WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}", rewardId)
    .FirstOrDefaultAsync();
```

### 8.3 Request Caching

Validator y Handler llaman a los mismos servicios (GetByIdAsync). Request Cache evita queries duplicados:

```
Validator:
  → CampaniaService.GetByIdAsync(id)
    → RequestCache MISS → DB query → Cache almacena

Handler:
  → CampaniaService.GetByIdAsync(id)
    → RequestCache HIT → Retorna inmediato (sin DB)
```

### 8.4 Cálculo Dinámico de Stock Vendido

NO almacenar `CantidadVendida` en tabla Reward (evitar inconsistencias). Calcular dinámicamente:

```csharp
var cantidadVendida = await _context.PedidoLineas
    .Where(l => l.RewardId == rewardId &&
                l.PedidoCrowdfunding.EstadoPedidoId == 3) // Solo COMPLETADOS
    .SumAsync(l => l.Cantidad);
```

**Optimización:** Cachear resultado con `RequestCacheService` (válido durante request actual).

### 8.5 Usuario Anónimo vs Autenticado

**Anónimo:**
- `UserId = null`
- `PermitirMostrarNombre = false` (siempre "Anónimo")
- Validar `campania.PermiteAportacionesAnonimas = true`

**Autenticado:**
- `UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)`
- `PermitirMostrarNombre = !esAnonimo` (checkbox en form)

### 8.6 Reward "Sin Recompensa"

Crear reward especial al publicar campaña:

```csharp
var defaultReward = new CampaniaCrowdfundingReward
{
    CampaniaId = campaniaId,
    TipoRewardId = 1,
    Nombre = "Apoyo sin recompensa",
    Descripcion = "Apoya el proyecto sin recibir recompensa",
    ImporteMinimo = 1.00m,
    MonedaId = 1,
    EsAddOn = false,
    CantidadMaxima = null,
    IncluyeEnvioFisico = false,
    Orden = 999,
    EsActivo = true
};
```

Frontend muestra este reward como botón especial "Apoyar sin recompensa" al final de la lista.

---

## 9. Queries Críticas para Performance

### Query 1: Detalle de Campaña con Rewards y Backings

```csharp
// CampaniaRepository.GetDetailByIdAsync
SELECT c.*, r.*, p.*
FROM CampaniaCrowdfunding c
LEFT JOIN CampaniaCrowdfundingReward r ON c.Id = r.CampaniaId
    WHERE r.EsActivo = 1
    ORDER BY r.Orden
LEFT JOIN PedidoCrowdfunding p ON c.Id = p.CampaniaId
    WHERE p.EstadoPedidoId = 3
    ORDER BY p.FechaCreacion DESC
    OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
WHERE c.Id = @campaniaId
```

**Índices necesarios:**
- `IX_CampaniaCrowdfundingReward_CampaniaId_EsActivo`
- `IX_PedidoCrowdfunding_CampaniaId_EstadoPedidoId_FechaCreacion`

### Query 2: Stock Vendido de Reward

```csharp
// RewardRepository.GetCantidadVendidaAsync
SELECT SUM(l.Cantidad)
FROM PedidoCrowdfundingLinea l
INNER JOIN PedidoCrowdfunding p ON l.PedidoCrowdfundingId = p.Id
WHERE l.RewardId = @rewardId
    AND p.EstadoPedidoId = 3
```

**Índices necesarios:**
- `IX_PedidoCrowdfundingLinea_RewardId`
- `IX_PedidoCrowdfunding_EstadoPedidoId`

### Query 3: Count Backers de Campaña

```csharp
// PedidoRepository.CountByCampaniaIdAsync
SELECT COUNT(*)
FROM PedidoCrowdfunding
WHERE CampaniaId = @campaniaId
    AND EstadoPedidoId = 3
```

**Índice necesario:**
- `IX_PedidoCrowdfunding_CampaniaId_EstadoPedidoId`

---

## 10. Siguiente Paso Sugerido

**Pasar a Application Layer (CQRS):**
- Crear `CreateBackingCommand` + Handler + Validator
- Crear `GetCampaniaDetailQuery` + Handler
- Crear `GetCampaniaStatsQuery` + Handler
- Crear AutoMapper profiles (BackingProfile, CampaniaProfile extensión)

**Archivo de plan:** `plans/hacer-backing/backend/cqrs-architecture.md`

---

## Notas Finales

- **NO implementar código** - Este es un plan arquitectónico detallado para guiar implementación
- **Handlers serán diseñados** en plan CQRS separado
- **Controllers serán diseñados** en plan API endpoints separado
- **Testing será diseñado** en plan de tests separado
- **Prioridad 1:** Transacción atómica en BackingService
- **Prioridad 2:** Optimistic Locking con RowVersion
- **Prioridad 3:** Request Cache para performance
