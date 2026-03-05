using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Interfaces;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Infra.Services;

/// <summary>
/// ArtistaService - Service para persistencia de Artista.
/// NOTA: UserAccessContext hereda de IdentityDbContext (no CoreDbContext),
/// por lo que no usa IUnitOfWork generico. El Repository hace SaveChanges directamente.
/// </summary>
public class ArtistaService : IArtistaService
{
    private readonly IArtistaRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<ArtistaService> _logger;

    public ArtistaService(
        IArtistaRepository repository,
        IRequestCacheService requestCache,
        ILogger<ArtistaService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Artista?> GetByIdAsync(ArtistaId id, CancellationToken ct)
    {
        var cacheKey = $"artista:{id.Value}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<Artista?> GetByUserIdAsync(string userId, CancellationToken ct)
    {
        var cacheKey = $"artista:user:{userId}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByUserIdAsync(userId, ct));
    }

    public async Task<bool> ExistsForUserAsync(string userId, CancellationToken ct)
    {
        var cacheKey = $"artista:exists:user:{userId}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.ExistsForUserAsync(userId, ct));
    }

    public async Task<ArtistaId> CreateAsync(Artista entity, CancellationToken ct)
    {
        entity.FechaCreacion = DateTime.UtcNow;
        var id = await _repository.AddAsync(entity, ct);

        _logger.LogInformation("Artista {ArtistaId} created for user {UserId}",
            id.Value, entity.UserIdPropietario);

        return id;
    }

    public async Task UpdateAsync(Artista entity, CancellationToken ct)
    {
        entity.FechaActualizacion = DateTime.UtcNow;
        await _repository.UpdateAsync(entity, ct);

        _logger.LogInformation("Artista {ArtistaId} updated", entity.Id.Value);
    }
}
