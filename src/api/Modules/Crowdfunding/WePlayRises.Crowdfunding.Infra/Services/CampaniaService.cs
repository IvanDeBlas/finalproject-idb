using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Infra.Services;

public class CampaniaService : ICampaniaService
{
    private readonly ICampaniaRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<CampaniaService> _logger;

    public CampaniaService(
        ICampaniaRepository repository,
        IRequestCacheService requestCache,
        ILogger<CampaniaService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CampaniaCrowdfunding?> GetByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct)
    {
        var cacheKey = $"campania:{id.Value}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<IReadOnlyList<CampaniaCrowdfunding>> GetAllAsync(CancellationToken ct)
    {
        return await _repository.GetAllAsync(ct);
    }

    public async Task<IReadOnlyList<CampaniaCrowdfunding>> GetByArtistaIdAsync(ArtistaId artistaId, CancellationToken ct)
    {
        var cacheKey = $"campanias:artista:{artistaId.Value}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByArtistaIdAsync(artistaId, ct))
            ?? Array.Empty<CampaniaCrowdfunding>();
    }

    public async Task<CampaniaCrowdfundingId> CreateAsync(CampaniaCrowdfunding campania, CancellationToken ct)
    {
        campania.FechaCreacion = DateTime.UtcNow;
        var id = await _repository.AddAsync(campania, ct);

        _logger.LogInformation("Campania {CampaniaId} created for Artista {ArtistaId}",
            id.Value, campania.ArtistaId.Value);

        return id;
    }

    public async Task UpdateAsync(CampaniaCrowdfunding campania, CancellationToken ct)
    {
        campania.FechaActualizacion = DateTime.UtcNow;
        await _repository.UpdateAsync(campania, ct);

        _logger.LogInformation("Campania {CampaniaId} updated", campania.Id.Value);
    }

    public async Task<bool> ExistsAsync(CampaniaCrowdfundingId id, CancellationToken ct)
    {
        var entity = await GetByIdAsync(id, ct);
        return entity != null;
    }

    public async Task<bool> ExistsByTituloAsync(string titulo, CancellationToken ct)
    {
        return await _repository.ExistsByTituloAsync(titulo, ct);
    }

    public async Task<bool> ExistsByTituloExcludingIdAsync(string titulo, CampaniaCrowdfundingId excludeId, CancellationToken ct)
    {
        return await _repository.ExistsByTituloExcludingIdAsync(titulo, excludeId, ct);
    }

    public async Task<CampaniaCrowdfunding?> GetDetailByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct)
    {
        return await _repository.GetDetailByIdAsync(id, ct);
    }

    public async Task<int> GetTotalBackersAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var cacheKey = $"campania:{campaniaId.Value}:backers";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.CountBackersByCampaniaIdAsync(campaniaId, ct));
    }
}
