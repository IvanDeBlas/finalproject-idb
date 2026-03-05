using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Interfaces;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Infra.Services;

public class PerfilProfesionalService : IPerfilProfesionalService
{
    private readonly IPerfilProfesionalRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<PerfilProfesionalService> _logger;

    public PerfilProfesionalService(
        IPerfilProfesionalRepository repository,
        IRequestCacheService requestCache,
        ILogger<PerfilProfesionalService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PerfilProfesional?> GetByUserIdAsync(string userId, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"perfilprofesional:user:{userId}",
            async () => await _repository.GetByUserIdAsync(userId, ct));
    }

    public async Task<bool> ExistsByUserIdAsync(string userId, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"perfilprofesional:exists:user:{userId}",
            async () => await _repository.ExistsByUserIdAsync(userId, ct));
    }
}
