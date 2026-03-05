# Arquitectura Hexagonal: Definir Recompensas

**Fecha:** 2026-02-13
**Modulo:** Crowdfunding
**Feature:** definir-recompensas

## 1. Resumen Ejecutivo

Esta feature permite a los artistas gestionar recompensas de sus campañas de crowdfunding mediante operaciones CRUD completas, incluyendo creación, actualización, eliminación (soft delete) y reordenamiento mediante drag & drop. El sistema valida ownership del artista, previene eliminación de recompensas con backings asociados, y gestiona automáticamente el orden de visualización.

**Estado Actual del Backend:**
- ✅ Entidad `CampaniaCrowdfundingReward` completamente definida con todos los campos necesarios
- ✅ `RewardRepository` con métodos básicos (GetById, GetAll, GetByCampaniaId, Add, Update)
- ✅ `RewardService` con cache, logging y métodos CRUD básicos
- ✅ `IRewardRepository` e `IRewardService` con interfaces básicas

**Lo que FALTA implementar:**
- ❌ Constante `BusinessRule_RewardHasBackings` (4010) en `ServiceResponseMessageType.cs`
- ❌ Métodos repository: `GetMaxOrdenAsync`, `UpdateBulkOrdenAsync`, `HasBackingsAsync`, `GetByIdWithLineasAsync`
- ❌ Métodos service: `GetMaxOrdenAsync`, `ReorderAsync`, `HasBackingsAsync`, validación ownership artista
- ❌ EF Core Configuration para `CampaniaCrowdfundingReward` (actualmente sin configuración explícita)

---

## 2. Domain Layer

### 2.1 Entidades

#### CampaniaCrowdfundingReward (EXISTENTE - No requiere cambios)
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Model/CampaniaCrowdfundingReward.cs`

**Estado:** ✅ COMPLETO - La entidad ya tiene todos los campos necesarios.

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | CampaniaCrowdfundingRewardId | No | PK (Strongly Typed ID) |
| CampaniaId | CampaniaCrowdfundingId | No | FK a CampaniaCrowdfunding |
| TipoRewardId | int | No | FK a MaestraTipoReward (1=Digital, 2=Fisico, 3=Experiencia, 4=Otro) |
| Nombre | string | No | Nombre de la recompensa (max 200) |
| Descripcion | string | Si | Descripción detallada (max 2000) |
| ImporteMinimo | decimal | No | Monto mínimo de aportación para obtener esta reward |
| MonedaId | int | No | FK a MaestraMoneda (1=EUR por defecto) |
| EsAddOn | bool | No | Si es add-on (item adicional opcional) |
| CantidadMaxima | int | Si | Stock máximo disponible (null = ilimitado) |
| CantidadPorBacker | int | Si | Cantidad máxima por backer |
| IncluyeEnvioFisico | bool | No | Si requiere dirección de envío |
| TiempoEntregaEstimado | string | Si | Texto libre de estimación entrega (max 200) |
| Orden | int | No | Orden de visualización (para drag & drop) |
| EsActivo | bool | No | Soft delete flag |
| FechaCreacion | DateTime | No | Timestamp de creación |
| FechaActualizacion | DateTime | Si | Timestamp de última actualización |

**Navegaciones:**
- `Campania` -> `CampaniaCrowdfunding` (N:1)
- `Lineas` -> `ICollection<PedidoCrowdfundingLinea>` (1:N) - Para validar backings asociados

**Computed Properties (NO agregar a entidad - calcular en DTOs):**
- `CantidadVendida` - Calculado desde `Lineas.Sum(l => l.Cantidad)` (futuro, en feature realizar-backing)
- `CantidadDisponible` - Calculado como `CantidadMaxima - CantidadVendida` (si CantidadMaxima != null)

### 2.2 Constants - ServiceResponseMessageType

#### MODIFICAR: ServiceResponseMessageType.cs
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Constants/ServiceResponseMessageType.cs`

**Acción:** Agregar nueva constante en sección Business Rule Errors (4000-4999).

**Nueva constante a agregar (línea ~62, después de `BusinessRule_CampaniaNotDraft`):**

```csharp
public const string BusinessRule_RewardHasBackings = "4010";
```

**Contexto de uso:**
- `DeleteRewardCommand` - Cuando se intenta eliminar reward con backings asociados
- `UpdateRewardCommand` - Cuando se intenta modificar campos críticos de reward con backings

---

## 3. Infrastructure Layer

### 3.1 Repository Interfaces

#### MODIFICAR: IRewardRepository
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Repositories/IRewardRepository.cs`

**Estado Actual:** Interface con métodos básicos CRUD.

**Métodos NUEVOS a agregar:**

| Método | Retorno | Parámetros | Descripción |
|--------|---------|------------|-------------|
| `GetByIdWithLineasAsync` | `Task<CampaniaCrowdfundingReward?>` | `CampaniaCrowdfundingRewardId id, CancellationToken ct` | Obtiene reward con navegación `Lineas` cargada (Include) para validar backings |
| `GetMaxOrdenAsync` | `Task<int>` | `CampaniaCrowdfundingId campaniaId, CancellationToken ct` | Obtiene el orden máximo de rewards de una campaña (para auto-incrementar) |
| `UpdateBulkOrdenAsync` | `Task` | `IEnumerable<(CampaniaCrowdfundingRewardId Id, int Orden)> updates, CancellationToken ct` | Actualiza orden de múltiples rewards en bulk (para reorder) |
| `HasBackingsAsync` | `Task<bool>` | `CampaniaCrowdfundingRewardId id, CancellationToken ct` | Verifica si reward tiene PedidoCrowdfundingLinea asociados |

**Interface completa resultante:**

```csharp
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Interfaces.Repositories;

public interface IRewardRepository
{
    // Métodos existentes
    Task<CampaniaCrowdfundingReward?> GetByIdAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct);
    Task<IReadOnlyList<CampaniaCrowdfundingReward>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<CampaniaCrowdfundingReward>> GetByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
    Task<CampaniaCrowdfundingRewardId> AddAsync(CampaniaCrowdfundingReward entity, CancellationToken ct);
    Task UpdateAsync(CampaniaCrowdfundingReward entity, CancellationToken ct);
    Task<bool> ExistsByNombreInCampaniaAsync(string nombre, CampaniaCrowdfundingId campaniaId, CancellationToken ct);
    Task<bool> ExistsByNombreInCampaniaExcludingIdAsync(string nombre, CampaniaCrowdfundingId campaniaId, CampaniaCrowdfundingRewardId excludeId, CancellationToken ct);

    // NUEVOS métodos para feature definir-recompensas
    Task<CampaniaCrowdfundingReward?> GetByIdWithLineasAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct);
    Task<int> GetMaxOrdenAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
    Task UpdateBulkOrdenAsync(IEnumerable<(CampaniaCrowdfundingRewardId Id, int Orden)> updates, CancellationToken ct);
    Task<bool> HasBackingsAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct);
}
```

---

### 3.2 Repository Implementations

#### MODIFICAR: RewardRepository
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Repositories/RewardRepository.cs`

**Estado Actual:** Repository con métodos básicos CRUD, ya implementa SaveChanges en cada operación.

**Métodos NUEVOS a implementar:**

```csharp
using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Domain.Model;
using WePlayRises.Crowdfunding.Infra.Context;

namespace WePlayRises.Crowdfunding.Infra.Repositories;

public class RewardRepository : IRewardRepository
{
    private readonly CrowdfundingContext _context;

    public RewardRepository(CrowdfundingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // ===== MÉTODOS EXISTENTES (mantener sin cambios) =====
    // GetByIdAsync, GetAllAsync, GetByCampaniaIdAsync, AddAsync, UpdateAsync,
    // ExistsByNombreInCampaniaAsync, ExistsByNombreInCampaniaExcludingIdAsync

    // ===== NUEVOS MÉTODOS =====

    /// <summary>
    /// Obtiene reward con navegación Lineas cargada (para validar backings asociados).
    /// </summary>
    public async Task<CampaniaCrowdfundingReward?> GetByIdWithLineasAsync(
        CampaniaCrowdfundingRewardId id,
        CancellationToken ct)
    {
        return await _context.Rewards
            .Include(r => r.Lineas)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    /// <summary>
    /// Obtiene el orden máximo de rewards de una campaña.
    /// Retorna 0 si no hay rewards (para auto-incrementar desde 1).
    /// </summary>
    public async Task<int> GetMaxOrdenAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var maxOrden = await _context.Rewards
            .Where(r => r.CampaniaId == campaniaId)
            .MaxAsync(r => (int?)r.Orden, ct);

        return maxOrden ?? 0;
    }

    /// <summary>
    /// Actualiza orden de múltiples rewards en una sola transacción (bulk update).
    /// Usado por ReorderRewardsCommand.
    /// </summary>
    public async Task UpdateBulkOrdenAsync(
        IEnumerable<(CampaniaCrowdfundingRewardId Id, int Orden)> updates,
        CancellationToken ct)
    {
        foreach (var (id, orden) in updates)
        {
            var reward = await _context.Rewards.FindAsync(new object[] { id }, ct);
            if (reward != null)
            {
                reward.Orden = orden;
                reward.FechaActualizacion = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Verifica si reward tiene backings (PedidoCrowdfundingLinea) asociados.
    /// Usado para prevenir eliminación de rewards con compromisos.
    /// </summary>
    public async Task<bool> HasBackingsAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct)
    {
        return await _context.PedidoLineas
            .AnyAsync(l => l.RewardId == id, ct);
    }
}
```

**IMPORTANTE - Repository hace SaveChanges:**
- ✅ `AddAsync` - SaveChanges al final
- ✅ `UpdateAsync` - SaveChanges al final
- ✅ `UpdateBulkOrdenAsync` - SaveChanges al final (después del foreach)
- ❌ `GetByIdWithLineasAsync` - Solo lectura, sin SaveChanges
- ❌ `GetMaxOrdenAsync` - Solo lectura, sin SaveChanges
- ❌ `HasBackingsAsync` - Solo lectura, sin SaveChanges

---

### 3.3 Service Interfaces

#### MODIFICAR: IRewardService
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Services/IRewardService.cs`

**Estado Actual:** Interface con métodos básicos CRUD.

**Métodos NUEVOS a agregar:**

| Método | Retorno | Parámetros | Descripción |
|--------|---------|------------|-------------|
| `GetByIdWithLineasAsync` | `Task<CampaniaCrowdfundingReward?>` | `CampaniaCrowdfundingRewardId id, CancellationToken ct` | Obtiene reward con backings cargados (sin cache, para validaciones en tiempo real) |
| `GetMaxOrdenAsync` | `Task<int>` | `CampaniaCrowdfundingId campaniaId, CancellationToken ct` | Obtiene orden máximo para auto-incrementar |
| `ReorderAsync` | `Task` | `CampaniaCrowdfundingId campaniaId, IEnumerable<(CampaniaCrowdfundingRewardId, int)> updates, CancellationToken ct` | Reordena múltiples rewards con validación de ownership |
| `HasBackingsAsync` | `Task<bool>` | `CampaniaCrowdfundingRewardId id, CancellationToken ct` | Verifica si reward tiene backings asociados |
| `ValidateOwnershipAsync` | `Task<bool>` | `CampaniaCrowdfundingRewardId rewardId, Guid artistaId, CancellationToken ct` | Valida que el artista es dueño de la campaña del reward |

**Interface completa resultante:**

```csharp
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Interfaces.Services;

public interface IRewardService
{
    // Métodos existentes
    Task<CampaniaCrowdfundingReward?> GetByIdAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct);
    Task<IReadOnlyList<CampaniaCrowdfundingReward>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<CampaniaCrowdfundingReward>> GetByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
    Task<CampaniaCrowdfundingRewardId> CreateAsync(CampaniaCrowdfundingReward reward, CancellationToken ct);
    Task UpdateAsync(CampaniaCrowdfundingReward reward, CancellationToken ct);
    Task<bool> ExistsAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct);
    Task<bool> ExistsByNombreInCampaniaAsync(string nombre, CampaniaCrowdfundingId campaniaId, CancellationToken ct);
    Task<bool> ExistsByNombreInCampaniaExcludingIdAsync(string nombre, CampaniaCrowdfundingId campaniaId, CampaniaCrowdfundingRewardId excludeId, CancellationToken ct);

    // NUEVOS métodos para feature definir-recompensas
    Task<CampaniaCrowdfundingReward?> GetByIdWithLineasAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct);
    Task<int> GetMaxOrdenAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
    Task ReorderAsync(CampaniaCrowdfundingId campaniaId, IEnumerable<(CampaniaCrowdfundingRewardId Id, int Orden)> updates, CancellationToken ct);
    Task<bool> HasBackingsAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct);
    Task<bool> ValidateOwnershipAsync(CampaniaCrowdfundingRewardId rewardId, Guid artistaId, CancellationToken ct);
}
```

---

### 3.4 Service Implementations

#### MODIFICAR: RewardService
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Services/RewardService.cs`

**Estado Actual:** Service con cache, logging, métodos CRUD básicos. ✅ Ya inyecta IRequestCacheService y ILogger correctamente.

**IMPORTANTE - Falta inyectar:**
- ❌ `ICampaniaRepository` o `ICampaniaService` - Para validar ownership del artista

**Métodos NUEVOS a implementar:**

```csharp
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Infra.Services;

public class RewardService : IRewardService
{
    private readonly IRewardRepository _repository;
    private readonly ICampaniaRepository _campaniaRepository; // NUEVO - Para validación ownership
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<RewardService> _logger;

    public RewardService(
        IRewardRepository repository,
        ICampaniaRepository campaniaRepository, // NUEVO
        IRequestCacheService requestCache,
        ILogger<RewardService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _campaniaRepository = campaniaRepository ?? throw new ArgumentNullException(nameof(campaniaRepository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ===== MÉTODOS EXISTENTES (mantener sin cambios) =====
    // GetByIdAsync, GetAllAsync, GetByCampaniaIdAsync, CreateAsync, UpdateAsync,
    // ExistsAsync, ExistsByNombreInCampaniaAsync, ExistsByNombreInCampaniaExcludingIdAsync

    // ===== NUEVOS MÉTODOS =====

    /// <summary>
    /// Obtiene reward con navegación Lineas cargada.
    /// NO usa cache (necesitamos datos en tiempo real para validaciones).
    /// </summary>
    public async Task<CampaniaCrowdfundingReward?> GetByIdWithLineasAsync(
        CampaniaCrowdfundingRewardId id,
        CancellationToken ct)
    {
        return await _repository.GetByIdWithLineasAsync(id, ct);
    }

    /// <summary>
    /// Obtiene orden máximo para auto-incrementar al crear nuevo reward.
    /// NO usa cache (necesitamos valor actualizado).
    /// </summary>
    public async Task<int> GetMaxOrdenAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        return await _repository.GetMaxOrdenAsync(campaniaId, ct);
    }

    /// <summary>
    /// Reordena múltiples rewards en una sola operación.
    /// Validación de ownership se hace en Handler (tiene acceso al artistaId del token).
    /// </summary>
    public async Task ReorderAsync(
        CampaniaCrowdfundingId campaniaId,
        IEnumerable<(CampaniaCrowdfundingRewardId Id, int Orden)> updates,
        CancellationToken ct)
    {
        await _repository.UpdateBulkOrdenAsync(updates, ct);

        _logger.LogInformation(
            "Reordered {Count} rewards for Campania {CampaniaId}",
            updates.Count(), campaniaId.Value);
    }

    /// <summary>
    /// Verifica si reward tiene backings asociados.
    /// Usado en DeleteRewardCommand para prevenir eliminación.
    /// </summary>
    public async Task<bool> HasBackingsAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct)
    {
        var cacheKey = $"reward:{id.Value}:hasBackings";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.HasBackingsAsync(id, ct));
    }

    /// <summary>
    /// Valida que el artista es dueño de la campaña del reward.
    /// Usado en todos los Commands para validar ownership antes de permitir operaciones.
    /// </summary>
    public async Task<bool> ValidateOwnershipAsync(
        CampaniaCrowdfundingRewardId rewardId,
        Guid artistaId,
        CancellationToken ct)
    {
        var reward = await GetByIdAsync(rewardId, ct);
        if (reward == null)
            return false;

        var campania = await _campaniaRepository.GetByIdAsync(reward.CampaniaId, ct);
        if (campania == null)
            return false;

        return campania.ArtistaId.Value == artistaId;
    }
}
```

**IMPORTANTE - Service retorna entidades, NO DTOs:**
- ✅ `GetByIdAsync` retorna `CampaniaCrowdfundingReward?`
- ✅ `GetByIdWithLineasAsync` retorna `CampaniaCrowdfundingReward?`
- ✅ `GetByCampaniaIdAsync` retorna `IReadOnlyList<CampaniaCrowdfundingReward>`
- ✅ `CreateAsync` retorna `CampaniaCrowdfundingRewardId` (ID de la entidad creada)
- ✅ `UpdateAsync` retorna `void` (no retorna la entidad actualizada)

**Handler hace el mapping a DTOs con AutoMapper.**

---

### 3.5 Entity Configurations

#### CREAR: CampaniaCrowdfundingRewardConfiguration
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Data/Configurations/CampaniaCrowdfundingRewardConfiguration.cs`

**Estado Actual:** ❌ NO EXISTE - La entidad NO tiene configuración explícita Fluent API.

**IMPORTANTE:** Usar Fluent API, NO Data Annotations.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Infra.Data.Configurations;

public class CampaniaCrowdfundingRewardConfiguration : IEntityTypeConfiguration<CampaniaCrowdfundingReward>
{
    public void Configure(EntityTypeBuilder<CampaniaCrowdfundingReward> builder)
    {
        // Table name
        builder.ToTable("CampaniaCrowdfundingReward");

        // Primary Key
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => new CampaniaCrowdfundingRewardId(value))
            .ValueGeneratedOnAdd();

        // Foreign Keys
        builder.Property(r => r.CampaniaId)
            .HasConversion(
                id => id.Value,
                value => new CampaniaCrowdfundingId(value))
            .IsRequired();

        builder.Property(r => r.TipoRewardId)
            .IsRequired();

        builder.Property(r => r.MonedaId)
            .IsRequired();

        // String properties
        builder.Property(r => r.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Descripcion)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(r => r.TiempoEntregaEstimado)
            .HasMaxLength(200)
            .IsRequired(false);

        // Numeric properties
        builder.Property(r => r.ImporteMinimo)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.CantidadMaxima)
            .IsRequired(false);

        builder.Property(r => r.CantidadPorBacker)
            .IsRequired(false);

        builder.Property(r => r.Orden)
            .IsRequired()
            .HasDefaultValue(0);

        // Boolean properties
        builder.Property(r => r.EsAddOn)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.IncluyeEnvioFisico)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.EsActivo)
            .IsRequired()
            .HasDefaultValue(true);

        // DateTime properties
        builder.Property(r => r.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(r => r.FechaActualizacion)
            .IsRequired(false);

        // Relationships
        builder.HasOne(r => r.Campania)
            .WithMany(c => c.Rewards)
            .HasForeignKey(r => r.CampaniaId)
            .OnDelete(DeleteBehavior.Restrict); // Prevenir cascada accidental

        builder.HasMany(r => r.Lineas)
            .WithOne(l => l.Reward)
            .HasForeignKey(l => l.RewardId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(r => r.CampaniaId)
            .HasDatabaseName("IX_CampaniaCrowdfundingReward_CampaniaId");

        builder.HasIndex(r => new { r.CampaniaId, r.Orden })
            .HasDatabaseName("IX_CampaniaCrowdfundingReward_CampaniaId_Orden");

        builder.HasIndex(r => new { r.CampaniaId, r.Nombre })
            .HasDatabaseName("IX_CampaniaCrowdfundingReward_CampaniaId_Nombre");
    }
}
```

**IMPORTANTE - Configuración en DbContext:**

El archivo de configuración debe aplicarse en `CrowdfundingContext.OnModelCreating()`:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // ... otras configuraciones ...

    // NUEVO - Aplicar configuración de Reward
    modelBuilder.ApplyConfiguration(new CampaniaCrowdfundingRewardConfiguration());
}
```

---

### 3.6 DbContext

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Context/CrowdfundingContext.cs`

**Acción:** Verificar que existe DbSet para Rewards y aplicar configuración.

**DbSet (probablemente YA EXISTE):**
```csharp
public DbSet<CampaniaCrowdfundingReward> Rewards => Set<CampaniaCrowdfundingReward>();
public DbSet<PedidoCrowdfundingLinea> PedidoLineas => Set<PedidoCrowdfundingLinea>();
```

**OnModelCreating (AGREGAR si falta):**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Aplicar todas las configuraciones del assembly
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrowdfundingContext).Assembly);
}
```

**IMPORTANTE:** Si el DbContext usa `ApplyConfigurationsFromAssembly`, la configuración de `CampaniaCrowdfundingRewardConfiguration` se aplicará automáticamente. Si no, agregar explícitamente:

```csharp
modelBuilder.ApplyConfiguration(new CampaniaCrowdfundingRewardConfiguration());
```

---

## 4. Dependency Injection

### 4.1 Estado Actual

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/DependencyInjection.cs` (o similar)

**Registros existentes (verificar):**
```csharp
services.AddScoped<IRewardRepository, RewardRepository>();
services.AddScoped<IRewardService, RewardService>();
```

### 4.2 Modificaciones Necesarias

**IMPORTANTE:** Si `RewardService` ahora inyecta `ICampaniaRepository`, verificar que `ICampaniaRepository` y `CampaniaRepository` estén registrados:

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddCrowdfundingServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<ICampaniaRepository, CampaniaRepository>();
        services.AddScoped<IRewardRepository, RewardRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();

        // Services
        services.AddScoped<ICampaniaService, CampaniaService>();
        services.AddScoped<IRewardService, RewardService>();
        services.AddScoped<IPedidoService, PedidoService>();

        return services;
    }
}
```

**No se requieren nuevos registros para esta feature** (todos los componentes ya deberían estar registrados).

---

## 5. Migraciones

### 5.1 Evaluación

**Estado Actual:**
- ✅ Entidad `CampaniaCrowdfundingReward` con todos los campos ya existe
- ❌ Configuración Fluent API nueva (`CampaniaCrowdfundingRewardConfiguration`)

**Decisión de Migraciones:**

**Opción 1 - Si la tabla YA EXISTE en DB:**
- La creación de `CampaniaCrowdfundingRewardConfiguration` NO generará migración nueva (solo organiza código)
- Los constraints e índices definidos en Fluent API se aplicarán si no existen
- Ejecutar: `dotnet ef database update` para aplicar cambios pendientes

**Opción 2 - Si la tabla NO EXISTE o hay cambios estructurales:**
- Crear migración: `dotnet ef migrations add AddRewardConfiguration --project src/api/Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra --startup-project src/api/WebApi`
- Aplicar: `dotnet ef database update --project src/api/WebApi`

### 5.2 Comandos

```bash
# Verificar migraciones pendientes
dotnet ef migrations list --project src/api/WebApi

# Crear nueva migración (si necesario)
dotnet ef migrations add AddRewardConfiguration \
  --project src/api/Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra \
  --startup-project src/api/WebApi \
  --context CrowdfundingContext

# Aplicar migraciones
dotnet ef database update --project src/api/WebApi --context CrowdfundingContext
```

**IMPORTANTE:** Verificar que el `--context` coincide con el nombre real del DbContext.

---

## 6. Archivos a Crear/Modificar

```
Modules/Crowdfunding/
├── WePlayRises.Crowdfunding.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs                [MODIFICAR] - Agregar BusinessRule_RewardHasBackings = "4010"
│
├── WePlayRises.Crowdfunding.Application/
│   └── Interfaces/
│       ├── Repositories/
│       │   └── IRewardRepository.cs                     [MODIFICAR] - Agregar 4 métodos nuevos
│       └── Services/
│           └── IRewardService.cs                        [MODIFICAR] - Agregar 5 métodos nuevos
│
└── WePlayRises.Crowdfunding.Infra/
    ├── Repositories/
    │   └── RewardRepository.cs                          [MODIFICAR] - Implementar 4 métodos nuevos
    ├── Services/
    │   └── RewardService.cs                             [MODIFICAR] - Implementar 5 métodos nuevos, inyectar ICampaniaRepository
    ├── Data/Configurations/
    │   └── CampaniaCrowdfundingRewardConfiguration.cs   [CREAR] - Fluent API completa
    └── Context/
        └── CrowdfundingContext.cs                       [VERIFICAR] - DbSet y ApplyConfiguration
```

---

## 7. Resumen de Cambios por Archivo

| Archivo | Tipo Cambio | Descripción |
|---------|-------------|-------------|
| `ServiceResponseMessageType.cs` | Modificar | Agregar constante `BusinessRule_RewardHasBackings = "4010"` en línea ~62 |
| `IRewardRepository.cs` | Modificar | Agregar 4 métodos: `GetByIdWithLineasAsync`, `GetMaxOrdenAsync`, `UpdateBulkOrdenAsync`, `HasBackingsAsync` |
| `IRewardService.cs` | Modificar | Agregar 5 métodos: `GetByIdWithLineasAsync`, `GetMaxOrdenAsync`, `ReorderAsync`, `HasBackingsAsync`, `ValidateOwnershipAsync` |
| `RewardRepository.cs` | Modificar | Implementar 4 métodos nuevos con EF Core queries (Include, Max, bulk update) |
| `RewardService.cs` | Modificar | Implementar 5 métodos nuevos, inyectar `ICampaniaRepository` en constructor |
| `CampaniaCrowdfundingRewardConfiguration.cs` | Crear | Nueva configuración Fluent API completa (tabla, PK, FK, propiedades, relaciones, índices) |
| `CrowdfundingContext.cs` | Verificar | Confirmar DbSet y aplicación de configuraciones (ApplyConfigurationsFromAssembly) |
| `DependencyInjection.cs` | Verificar | Confirmar que ICampaniaRepository/CampaniaRepository están registrados |

---

## 8. Validaciones y Reglas de Negocio

### 8.1 Validación de Ownership

**Flujo completo:**
1. Usuario autenticado envía request a endpoint protegido (POST/PUT/DELETE/PUT reorder)
2. Token JWT contiene claim `sub` con el ArtistaId (Guid)
3. Handler extrae ArtistaId del token: `var artistaId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))`
4. Handler llama `_rewardService.ValidateOwnershipAsync(rewardId, artistaId, ct)`
5. Service obtiene el reward, luego su campaña, y compara `campania.ArtistaId == artistaId`
6. Si NO match → Handler retorna `ServiceResponse` con error `Auth_Forbidden` (3002)

**Implementación en Handler (pseudocódigo):**
```csharp
// En CreateRewardCommandHandler, UpdateRewardCommandHandler, DeleteRewardCommandHandler, ReorderRewardsCommandHandler
var artistaId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

// Para Create: validar ownership de campania directamente
var campania = await _campaniaService.GetByIdAsync(request.CampaniaId, ct);
if (campania == null || campania.ArtistaId.Value != artistaId)
{
    return new ServiceResponse<Guid>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "No tienes permiso para crear recompensas en esta campaña", ErrorCode = ServiceResponseMessageType.Auth_Forbidden }
        }
    };
}

// Para Update/Delete: validar ownership del reward
var isOwner = await _rewardService.ValidateOwnershipAsync(request.Id, artistaId, ct);
if (!isOwner)
{
    return new ServiceResponse<bool>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "No tienes permiso para modificar esta recompensa", ErrorCode = ServiceResponseMessageType.Auth_Forbidden }
        }
    };
}
```

### 8.2 Validación de Backings

**Flujo completo:**
1. Usuario intenta eliminar reward (`DELETE /api/rewards/{id}`)
2. Handler llama `_rewardService.HasBackingsAsync(rewardId, ct)`
3. Service verifica si existen `PedidoCrowdfundingLinea` con `RewardId == rewardId`
4. Si tiene backings → Handler retorna error `BusinessRule_RewardHasBackings` (4010)
5. Si NO tiene backings → Procede con soft delete (`EsActivo = false`)

**Implementación en DeleteRewardCommandHandler (pseudocódigo):**
```csharp
// Validar que reward no tiene backings
var hasBackings = await _rewardService.HasBackingsAsync(request.Id, ct);
if (hasBackings)
{
    return new ServiceResponse<bool>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "No se puede eliminar una recompensa con aportes existentes", ErrorCode = ServiceResponseMessageType.BusinessRule_RewardHasBackings }
        }
    };
}

// Soft delete
var reward = await _rewardService.GetByIdAsync(request.Id, ct);
reward.EsActivo = false;
await _rewardService.UpdateAsync(reward, ct);
```

### 8.3 Auto-incrementar Orden

**Flujo completo:**
1. Usuario crea nueva recompensa (`POST /api/rewards`)
2. Handler NO incluye `Orden` en request (o incluye 0)
3. Handler llama `_rewardService.GetMaxOrdenAsync(campaniaId, ct)` → Retorna ej. 3
4. Handler asigna `reward.Orden = maxOrden + 1` → Orden = 4
5. Service persiste reward con orden auto-incrementado

**Implementación en CreateRewardCommandHandler (pseudocódigo):**
```csharp
// Auto-incrementar orden
var maxOrden = await _rewardService.GetMaxOrdenAsync(request.CampaniaId, ct);
var reward = _mapper.Map<CampaniaCrowdfundingReward>(request);
reward.Orden = maxOrden + 1;
reward.EsActivo = true;

var id = await _rewardService.CreateAsync(reward, ct);
```

### 8.4 Reordenamiento Bulk

**Flujo completo:**
1. Usuario reordena rewards con drag & drop en frontend
2. Frontend envía `PUT /api/rewards/reorder` con `{ campaniaId, rewardOrders: [{rewardId, orden}] }`
3. Handler valida ownership de campaña
4. Handler llama `_rewardService.ReorderAsync(campaniaId, updates, ct)`
5. Service llama `_repository.UpdateBulkOrdenAsync(updates, ct)`
6. Repository actualiza orden de cada reward en foreach, SaveChanges al final

**IMPORTANTE:** Repository hace una sola transacción para todos los updates (eficiencia).

---

## 9. Consideraciones de Performance

### 9.1 Queries con Include

**Métodos que cargan navegaciones:**
- `GetByIdWithLineasAsync` - Carga `Lineas` (collection) para validar backings
- Usar **SOLO cuando sea necesario** (validaciones en tiempo real)
- NO usar para queries de listado (overhead innecesario)

**Optimización:**
```csharp
// CORRECTO - Include solo para validaciones específicas
var reward = await _repository.GetByIdWithLineasAsync(id, ct);
if (reward.Lineas.Any())
{
    // Tiene backings
}

// EVITAR - Include en queries de listado
var rewards = await _context.Rewards
    .Include(r => r.Lineas) // NO NECESARIO para listas
    .ToListAsync(ct);
```

### 9.2 Request Caching

**Métodos con cache:**
- ✅ `GetByIdAsync` - Cache por `reward:{id}`
- ✅ `GetByCampaniaIdAsync` - Cache por `rewards:campania:{campaniaId}`
- ✅ `HasBackingsAsync` - Cache por `reward:{id}:hasBackings`

**Métodos SIN cache (datos en tiempo real):**
- ❌ `GetByIdWithLineasAsync` - Necesitamos estado actual de backings
- ❌ `GetMaxOrdenAsync` - Necesitamos orden máximo actualizado
- ❌ `UpdateAsync`, `ReorderAsync` - Operaciones de escritura

**Beneficio:** Validators y Handlers comparten datos via cache, evitando queries duplicados a DB en el mismo request.

### 9.3 Bulk Updates

**Método `UpdateBulkOrdenAsync`:**
- Actualiza múltiples rewards en una sola transacción
- Evita N+1 queries (un SaveChanges al final, no uno por reward)
- Optimización para drag & drop con muchas recompensas

---

## 10. Checklist de Implementación

### Domain Layer
- [ ] Entidad `CampaniaCrowdfundingReward` verificada (✅ YA COMPLETA, no requiere cambios)
- [ ] Constante `BusinessRule_RewardHasBackings = "4010"` agregada en `ServiceResponseMessageType.cs`

### Infrastructure - Interfaces
- [ ] `IRewardRepository` actualizado con 4 métodos nuevos
- [ ] `IRewardService` actualizado con 5 métodos nuevos

### Infrastructure - Implementations
- [ ] `RewardRepository` implementa 4 métodos nuevos con SaveChanges
- [ ] `RewardService` implementa 5 métodos nuevos, inyecta `ICampaniaRepository`
- [ ] `CampaniaCrowdfundingRewardConfiguration` creada con Fluent API completa
- [ ] `CrowdfundingContext` aplica configuración (ApplyConfigurationsFromAssembly)

### Dependency Injection
- [ ] `ICampaniaRepository` y `CampaniaRepository` registrados en DI
- [ ] `IRewardRepository` y `RewardRepository` registrados en DI
- [ ] `IRewardService` y `RewardService` registrados en DI

### Migraciones
- [ ] Migración creada (si necesario) para configuración nueva
- [ ] Migración aplicada a DB con `dotnet ef database update`

### Validaciones
- [ ] Repository hace SaveChanges en operaciones de escritura
- [ ] Service retorna entidades, NO DTOs
- [ ] Service inyecta IRequestCacheService para cache
- [ ] Service usa cache solo donde corresponde (no en validaciones en tiempo real)
- [ ] Todos los constructores usan `?? throw new ArgumentNullException`

---

## 11. Siguiente Paso Sugerido

Una vez implementada esta arquitectura hexagonal:

1. **CQRS Planning Architect** - Diseñar Commands/Queries:
   - `CreateRewardCommand` + Handler + Validator
   - `UpdateRewardCommand` + Handler + Validator
   - `DeleteRewardCommand` + Handler + Validator
   - `ReorderRewardsCommand` + Handler + Validator
   - `GetRewardByIdQuery` + Handler
   - `GetAllRewardsQuery` + Handler (con filtros campaniaId, esActivo)

2. **API Design Architect** - Diseñar endpoints en `RewardsController`:
   - `POST /api/rewards`
   - `GET /api/rewards?campaniaId={id}`
   - `GET /api/rewards/{id}`
   - `PUT /api/rewards/{id}`
   - `DELETE /api/rewards/{id}`
   - `PUT /api/rewards/reorder`

3. **Implementación de Application Layer** - Implementar Commands, Queries, Validators, Handlers usando los Services y Repositories diseñados aquí.

---

**Puntos clave de esta arquitectura:**
- Entidad `CampaniaCrowdfundingReward` ya completa, no requiere cambios
- Repository implementa métodos especializados (bulk update, max orden, has backings)
- Service valida ownership usando `ICampaniaRepository`
- Configuración Fluent API centralizada para mantener dominio POCO
- Request caching estratégico (solo donde aporta valor, no en validaciones en tiempo real)
- Soft delete con validación de backings asociados

---

**Fin del plan de arquitectura hexagonal.**
