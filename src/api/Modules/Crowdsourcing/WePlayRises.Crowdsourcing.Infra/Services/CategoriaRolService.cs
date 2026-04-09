using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Services;

public class CategoriaRolService : ICategoriaRolService
{
    private readonly IMaestraCategoriaRolRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<CategoriaRolService> _logger;

    public CategoriaRolService(
        IMaestraCategoriaRolRepository repository,
        IRequestCacheService requestCache,
        ILogger<CategoriaRolService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<MaestraCategoriaRol?> GetByIdAsync(int id, CancellationToken ct)
    {
        var cacheKey = $"categoria-rol:{id}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<IReadOnlyList<MaestraCategoriaRol>> GetAllAsync(CancellationToken ct)
    {
        var cacheKey = "categorias-rol:all";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetAllAsync(ct))
            ?? Array.Empty<MaestraCategoriaRol>();
    }
}
