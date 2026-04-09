using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Services;

public class RolProfesionalService : IRolProfesionalService
{
    private readonly IMaestraRolProfesionalRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<RolProfesionalService> _logger;

    public RolProfesionalService(
        IMaestraRolProfesionalRepository repository,
        IRequestCacheService requestCache,
        ILogger<RolProfesionalService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<MaestraRolProfesional?> GetByIdAsync(int id, CancellationToken ct)
    {
        var cacheKey = $"rol-profesional:{id}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<IReadOnlyList<MaestraRolProfesional>> GetAllActivosAsync(CancellationToken ct)
    {
        var cacheKey = "roles-profesionales:activos";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetAllActivosAsync(ct))
            ?? Array.Empty<MaestraRolProfesional>();
    }

    public async Task<IReadOnlyList<MaestraRolProfesional>> GetByCategoriaIdAsync(int categoriaId, CancellationToken ct)
    {
        var cacheKey = $"roles-profesionales:categoria:{categoriaId}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByCategoriaIdAsync(categoriaId, ct))
            ?? Array.Empty<MaestraRolProfesional>();
    }
}
