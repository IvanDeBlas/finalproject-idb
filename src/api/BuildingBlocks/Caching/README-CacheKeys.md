# CacheKeys Registry - Usage Guide

## Overview

The `CacheKeys` registry provides a centralized, compile-time safe way to manage cache keys across the application. This prevents magic strings, reduces errors, and makes cache key management maintainable.

## Benefits

✅ **Compile-time safety** - Typos are caught at build time
✅ **IntelliSense support** - Easy discovery of available cache keys
✅ **Consistent naming** - Enforces naming conventions
✅ **Easy refactoring** - Change keys in one place
✅ **Documentation** - Self-documenting code

## Migration Guide

### ❌ BEFORE (Magic Strings)

```csharp
// Service with magic strings
public class TipoGastoService : ITipoGastoService
{
    private readonly IMemoryCacheProvider _cache;

    public List<TipoGasto> GetAll()
    {
        // ❌ Magic string - prone to typos
        var cached = _cache.Get<List<TipoGasto>>("MaestraTipoGastoCache");
        if (cached != null) return cached;

        var data = _repository.GetAll();

        // ❌ Magic string repeated - hard to maintain
        _cache.Set(data, "MaestraTipoGastoCache");

        return data;
    }

    public async Task<int> CreateAsync(TipoGasto entity)
    {
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        // ❌ Magic string again - risk of inconsistency
        _cache.Remove("MaestraTipoGastoCache");

        return entity.Id;
    }
}
```

### ✅ AFTER (CacheKeys Registry)

```csharp
using mU.Cloud.Platform.Caching.Constants;

// Service with CacheKeys
public class TipoGastoService : ITipoGastoService
{
    private readonly IMemoryCacheProvider _cache;

    public List<TipoGasto> GetAll()
    {
        // ✅ Compile-time safe, IntelliSense support
        var cached = _cache.Get<List<TipoGasto>>(CacheKeys.Maestras.TipoGasto);
        if (cached != null) return cached;

        var data = _repository.GetAll();

        // ✅ Consistent key usage
        _cache.Set(data, CacheKeys.Maestras.TipoGasto);

        return data;
    }

    public async Task<int> CreateAsync(TipoGasto entity)
    {
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        // ✅ Easy to refactor, no risk of typos
        _cache.Remove(CacheKeys.Maestras.TipoGasto);

        return entity.Id;
    }
}
```

## Usage Examples

### 1. Master Data Tables (Maestras)

```csharp
// Get all TipoEvento from cache
var tiposEvento = _cache.Get<List<TipoEvento>>(CacheKeys.Maestras.TipoEvento);

// Set Provincias in cache
_cache.Set(provincias, CacheKeys.Maestras.Provincia);

// Invalidate TipoIncidencia cache
_cache.Remove(CacheKeys.Maestras.TipoIncidencia);
```

### 2. Entity-Specific Caches with IDs

```csharp
// Cache a specific Finca by ID
var fincaId = "abc123";
var cacheKey = CacheKeys.Fincas.ById(fincaId);
_cache.Set(finca, cacheKey);

// Cache Vivienda saldo
var viviendaId = "xyz789";
var saldo = _cache.Get<decimal>(CacheKeys.Viviendas.Saldo(viviendaId));

// Cache user roles
var userId = "user123";
_cache.Set(roles, CacheKeys.Users.UserRoles(userId));
```

### 3. Related Entity Collections

```csharp
// Cache all Viviendas for a Finca
var fincaId = "finca123";
var viviendas = await _service.GetViviendasByFincaIdAsync(fincaId);
_cache.Set(viviendas, CacheKeys.Fincas.Viviendas(fincaId));

// Cache Instalacion reservas
var instalacionId = "inst456";
var reservas = await _service.GetReservasAsync(instalacionId);
_cache.Set(reservas, CacheKeys.Instalaciones.Reservas(instalacionId));
```

### 4. Paginated Data

```csharp
// Cache paginated movimientos
var viviendaId = "viv123";
var pageNumber = 1;
var pageSize = 10;

var cacheKey = CacheKeys.Helpers.Paginated(
    CacheKeys.Viviendas.Movimientos(viviendaId),
    pageNumber,
    pageSize
);

var pagedData = _cache.Get<PagedList<MovimientoDto>>(cacheKey);
```

### 5. Versioned Cache (for breaking changes)

```csharp
// Cache with version for cache busting
var version = "2.1.0";
var baseKey = CacheKeys.Configuration.AppSettings;
var versionedKey = CacheKeys.Helpers.Versioned(baseKey, version);

_cache.Set(settings, versionedKey);
```

### 6. Pattern-Based Invalidation

```csharp
// Invalidate all Finca-related caches (requires cache provider support)
var pattern = CacheKeys.Helpers.Pattern("fincas:");
_cache.RemoveByPattern(pattern); // Removes fincas:list, fincas:abc123, etc.

// Invalidate all user-specific caches
var userPattern = CacheKeys.Helpers.Pattern($"users:{userId}");
_cache.RemoveByPattern(userPattern);
```

## Updating FullMasterTableService

The `FullMasterTableService<TEntity>` base class can be updated to use CacheKeys:

### Current Implementation (Dynamic)
```csharp
public class FullMasterTableService<TEntity> where TEntity : class
{
    private readonly string _cache;

    public FullMasterTableService(/* ... */)
    {
        // Dynamic cache key based on entity type
        _cache = $"{typeof(TEntity).Name}Cache";
    }
}
```

### Option 1: Keep Dynamic (Recommended for Base Class)
The current implementation is actually good for the generic base class since it works for any entity type. Just document that specific services can override with CacheKeys.

### Option 2: Override in Derived Services
```csharp
public class TipoGastoService : FullMasterTableService<TipoGasto>
{
    // Override cache key with registry
    protected override string GetCacheKey() => CacheKeys.Maestras.TipoGasto;
}
```

## Best Practices

### ✅ DO

- **Always use CacheKeys** for all cache operations
- **Add new keys** to the registry when adding new cache operations
- **Group related keys** in nested static classes by module/domain
- **Use helper methods** for dynamic keys (ById, Paginated, etc.)
- **Document cache keys** with XML comments when purpose is not obvious

### ❌ DON'T

- **Don't use magic strings** for cache keys
- **Don't create duplicate keys** - reuse existing ones
- **Don't hardcode IDs** in key names - use methods like `ById(string id)`
- **Don't forget to invalidate** related caches after updates

## Cache Invalidation Strategies

### Strategy 1: Direct Invalidation
```csharp
// After creating a new Finca, invalidate list cache
await _repository.AddAsync(finca);
await _unitOfWork.SaveChangesAsync();

_cache.Remove(CacheKeys.Fincas.List);
```

### Strategy 2: Multiple Related Caches
```csharp
// After updating a Vivienda, invalidate multiple caches
await _repository.UpdateAsync(vivienda);
await _unitOfWork.SaveChangesAsync();

_cache.Remove(CacheKeys.Viviendas.ById(vivienda.Id));
_cache.Remove(CacheKeys.Fincas.Viviendas(vivienda.FincaId));
_cache.Remove(CacheKeys.Viviendas.Saldo(vivienda.Id));
```

### Strategy 3: Pattern-Based Invalidation
```csharp
// After any Finca change, invalidate all Finca caches
var pattern = CacheKeys.Helpers.Pattern("fincas:");
_cache.RemoveByPattern(pattern);
```

## Testing

### Unit Test Example
```csharp
[Fact]
public async Task CreateAsync_ShouldInvalidateCache()
{
    // Arrange
    var cacheMock = new Mock<IMemoryCacheProvider>();
    var service = new TipoGastoService(cacheMock.Object, /* ... */);
    var entity = new TipoGasto { Nombre = "Test" };

    // Act
    await service.CreateAsync(entity);

    // Assert - Verify correct cache key was removed
    cacheMock.Verify(
        c => c.Remove(CacheKeys.Maestras.TipoGasto),
        Times.Once
    );
}
```

## Integration with Request Cache

For request-scoped caching (IRequestCacheService), you can also use CacheKeys:

```csharp
public class FincaService : IFincaService
{
    private readonly IRequestCacheService _requestCache;
    private readonly IFincaRepository _repository;

    public async Task<Finca?> GetByIdAsync(string id, CancellationToken ct)
    {
        // Use CacheKeys for request-scoped cache
        var cacheKey = CacheKeys.Fincas.ById(id);

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            () => _repository.GetByIdAsync(id, ct)
        );
    }
}
```

## Summary

The CacheKeys registry is a simple but powerful pattern that:
- ✅ Eliminates magic strings
- ✅ Provides compile-time safety
- ✅ Improves code maintainability
- ✅ Makes cache management discoverable
- ✅ Reduces bugs from typos

**Always use CacheKeys.** for any cache operation in the application.
