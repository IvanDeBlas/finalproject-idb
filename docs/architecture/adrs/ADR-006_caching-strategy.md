# ADR-006: Estrategia de Caching para Optimizacion de Requests

## Metadata
- **Estado**: Aceptada
- **Fecha**: 2026-01-23
- **Relacionado con**: ADR-002 (CQRS)
- **Basado en**: miUrba caching patterns

---

## Contexto

En el flujo CQRS, frecuentemente ocurren situaciones donde:
1. El **Validator** necesita verificar la existencia de una entidad
2. El **Handler** necesita la misma entidad para procesarla
3. Sin caching, esto resulta en queries duplicados a la base de datos

Ademas, hay casos donde validaciones complejas calculan datos (permisos, reglas de negocio) que el Handler necesita reutilizar.

---

## Decision

Implementar **dos niveles de caching** manejados exclusivamente desde la capa de **Services**:

### 1. Request-Scoped Cache (`IRequestCacheService`)

Cache efimero que vive durante un unico HTTP request. Ideal para:
- Evitar queries duplicados entre Validator y Handler
- Compartir resultados de calculos complejos dentro del mismo request

### 2. Memory Cache (`IMemoryCacheProvider`)

Cache persistente con TTL configurable. Para datos que:
- Cambian poco frecuentemente
- Son costosos de obtener
- Se acceden frecuentemente entre requests

---

## Regla Critica: Solo Services Usan Cache

```
❌ Handler -> IRequestCacheService      // PROHIBIDO
❌ Validator -> IRequestCacheService    // PROHIBIDO
✅ Handler -> Service -> Cache          // CORRECTO
✅ Validator -> Service -> Cache        // CORRECTO
```

**Razon**: Mantiene la responsabilidad de caching encapsulada en una sola capa, evitando inconsistencias y facilitando testing.

---

## Interfaces

### IRequestCacheService (Request-Scoped)

```csharp
public interface IRequestCacheService
{
    bool TryGet<T>(string key, out T? value);
    Task<T?> GetOrAddAsync<T>(string key, Func<Task<T?>> factory);
    void Set<T>(string key, T value);
}
```

**Ciclo de vida**: Scoped (se crea y destruye con cada HTTP request)

### IMemoryCacheProvider (Persistent)

```csharp
public interface IMemoryCacheProvider
{
    TEntity? Get<TEntity>(string key);
    TEntity Set<TEntity>(TEntity entity, string key);
    TEntity Set<TEntity>(TEntity entity, string key, TimeSpan timeSpan);
    IEnumerable<TEntity>? GetCollection<TEntity>(string key);
    void Remove(string key);
    void ClearCache();
}
```

**Ciclo de vida**: Singleton (persiste durante toda la vida de la aplicacion)

---

## Ejemplo de Implementacion

### Escenario: Validator verifica artista, Handler lo necesita

**ArtistaService.cs:**

```csharp
public class ArtistaService : IArtistaService
{
    private readonly IArtistaRepository _repository;
    private readonly IRequestCacheService _requestCache;

    public ArtistaService(
        IArtistaRepository repository,
        IRequestCacheService requestCache)
    {
        _repository = repository;
        _requestCache = requestCache;
    }

    public async Task<Artista?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var cacheKey = $"artista:{id}";

        // GetOrAddAsync: si existe en cache lo retorna, si no lo busca y cachea
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id, ct));
    }
}
```

**CreateCampaniaValidator.cs:**

```csharp
public class CreateCampaniaValidator : AbstractValidator<CreateCampaniaCommand>
{
    private readonly IArtistaService _artistaService;

    public CreateCampaniaValidator(IArtistaService artistaService)
    {
        _artistaService = artistaService;

        RuleFor(x => x.ArtistaId)
            .MustAsync(ArtistaExistsAsync)
            .WithMessage("El artista no existe")
            .WithErrorCode("ARTISTA_NOT_FOUND");
    }

    private async Task<bool> ArtistaExistsAsync(Guid artistaId, CancellationToken ct)
    {
        // Primera llamada: va a DB y cachea
        var artista = await _artistaService.GetByIdAsync(artistaId, ct);
        return artista != null;
    }
}
```

**CreateCampaniaHandler.cs:**

```csharp
public async Task<ServiceResponse<Guid>> Handle(
    CreateCampaniaCommand request,
    CancellationToken ct)
{
    // Segunda llamada: obtiene del cache (NO va a DB)
    var artista = await _artistaService.GetByIdAsync(request.ArtistaId, ct);

    // artista ya no es null porque el Validator lo verifico
    var campania = new Campania
    {
        ArtistaId = artista!.Id,
        Titulo = request.Titulo,
        // ...
    };

    // ...
}
```

**Flujo:**
```
Request HTTP
    |
    v
Validator.ArtistaExistsAsync()
    |-> ArtistaService.GetByIdAsync()
        |-> RequestCache.GetOrAddAsync() --> Cache MISS --> DB Query
        |-> Almacena en cache
    |
    v
Handler.Handle()
    |-> ArtistaService.GetByIdAsync()
        |-> RequestCache.GetOrAddAsync() --> Cache HIT --> Retorna inmediato
    |
    v
Response
```

---

## Convencion de Cache Keys

Usar formato consistente: `{entidad}:{id}` o `{entidad}:{id}:{propiedad}`

```csharp
// Ejemplos
$"artista:{artistaId}"
$"campania:{campaniaId}"
$"campania:{campaniaId}:rewards"
$"usuario:{userId}:permisos"
```

---

## Cuando Usar Cada Cache

| Escenario | Cache a Usar |
|-----------|--------------|
| Validar existencia de entidad que Handler usara | `IRequestCacheService` |
| Calculos de permisos usados en Validator y Handler | `IRequestCacheService` |
| Datos de referencia que cambian poco (paises, categorias) | `IMemoryCacheProvider` |
| Configuraciones de aplicacion | `IMemoryCacheProvider` |
| Datos especificos de un request | `IRequestCacheService` |

---

## Justificacion

- **Rendimiento**: Reduce queries duplicados significativamente
- **Consistencia**: Garantiza que Validator y Handler ven los mismos datos
- **Encapsulamiento**: Cache manejado en Services, no expuesto a capas superiores
- **Testing**: Facil de mockear `IRequestCacheService` en tests

---

## Alternativas Consideradas

### Alternativa 1: Caching en Handlers
- **Descripcion**: Handler cachea resultados localmente
- **Razon para no elegirla**: Duplica logica, Validator no se beneficia

### Alternativa 2: Caching via MediatR Pipeline
- **Descripcion**: Behavior de MediatR que cachea requests completos
- **Razon para no elegirla**: Granularidad muy gruesa, no aplica a validaciones

### Alternativa 3: Sin caching
- **Descripcion**: Cada llamada va a DB
- **Razon para no elegirla**: Rendimiento suboptimo, queries innecesarios

---

## Consecuencias

### Positivas
- Reduccion de queries a DB (tipicamente 30-50% menos en operaciones CRUD)
- Validator y Handler comparten datos sin conocerse
- Facil de agregar caching a Services existentes

### Negativas
- Complejidad adicional en Services
- Necesidad de gestionar invalidacion para MemoryCacheProvider

### Riesgos
- **Cache stale con MemoryCacheProvider**: Usar TTL apropiados, invalidar en mutaciones
- **Memory leaks**: RequestCacheService se limpia automaticamente, MemoryCacheProvider necesita configurar MaxCacheSize

---

## Notas de Implementacion

**Registro en DI:**

```csharp
// RequestCacheService es Scoped (por request)
services.AddScoped<IRequestCacheService, RequestCacheService>();

// MemoryCacheProvider es Singleton
services.AddSingleton<IMemoryCacheProvider, MemoryCacheProvider>();
```

**CacheOptions por defecto:**

```csharp
public class CacheOptions
{
    public uint CacheItemTtl { get; set; } = 1800000;     // 30 minutos
    public ushort MaxCacheSize { get; set; } = 1024;      // 1024 items
}
```

---

## Referencias

- [Caching Best Practices - Microsoft](https://docs.microsoft.com/en-us/azure/architecture/best-practices/caching)
- [Request-Scoped Services in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- ADR-002: CQRS con MediatR
