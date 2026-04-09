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
    private readonly ICampaniaRepository _campaniaRepository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<RewardService> _logger;

    public RewardService(
        IRewardRepository repository,
        ICampaniaRepository campaniaRepository,
        IRequestCacheService requestCache,
        ILogger<RewardService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _campaniaRepository = campaniaRepository ?? throw new ArgumentNullException(nameof(campaniaRepository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CampaniaCrowdfundingReward?> GetByIdAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct)
    {
        var cacheKey = $"reward:{id.Value}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<IReadOnlyList<CampaniaCrowdfundingReward>> GetAllAsync(CancellationToken ct)
    {
        return await _repository.GetAllAsync(ct);
    }

    public async Task<IReadOnlyList<CampaniaCrowdfundingReward>> GetByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var cacheKey = $"rewards:campania:{campaniaId.Value}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByCampaniaIdAsync(campaniaId, ct))
            ?? Array.Empty<CampaniaCrowdfundingReward>();
    }

    public async Task<CampaniaCrowdfundingRewardId> CreateAsync(CampaniaCrowdfundingReward reward, CancellationToken ct)
    {
        reward.FechaCreacion = DateTime.UtcNow;
        var id = await _repository.AddAsync(reward, ct);

        _logger.LogInformation("Reward {RewardId} created for Campania {CampaniaId}",
            id.Value, reward.CampaniaId.Value);

        return id;
    }

    public async Task UpdateAsync(CampaniaCrowdfundingReward reward, CancellationToken ct)
    {
        reward.FechaActualizacion = DateTime.UtcNow;
        await _repository.UpdateAsync(reward, ct);

        _logger.LogInformation("Reward {RewardId} updated", reward.Id.Value);
    }

    public async Task<bool> ExistsAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct)
    {
        var entity = await GetByIdAsync(id, ct);
        return entity != null;
    }

    public async Task<bool> ExistsByNombreInCampaniaAsync(string nombre, CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        return await _repository.ExistsByNombreInCampaniaAsync(nombre, campaniaId, ct);
    }

    public async Task<bool> ExistsByNombreInCampaniaExcludingIdAsync(string nombre, CampaniaCrowdfundingId campaniaId, CampaniaCrowdfundingRewardId excludeId, CancellationToken ct)
    {
        return await _repository.ExistsByNombreInCampaniaExcludingIdAsync(nombre, campaniaId, excludeId, ct);
    }

    public async Task<CampaniaCrowdfundingReward?> GetByIdWithLineasAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct)
    {
        return await _repository.GetByIdWithLineasAsync(id, ct);
    }

    public async Task<int> GetMaxOrdenAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        return await _repository.GetMaxOrdenAsync(campaniaId, ct);
    }

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

    public async Task<bool> HasBackingsAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct)
    {
        var cacheKey = $"reward:{id.Value}:hasBackings";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.HasBackingsAsync(id, ct));
    }

    public async Task<bool> ValidateOwnershipAsync(CampaniaCrowdfundingRewardId rewardId, Guid artistaId, CancellationToken ct)
    {
        var reward = await GetByIdAsync(rewardId, ct);
        if (reward == null)
            return false;

        var campania = await _campaniaRepository.GetByIdAsync(reward.CampaniaId, ct);
        if (campania == null)
            return false;

        return campania.ArtistaId.Value == artistaId;
    }

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

        if (reward.CantidadMaxima == null) return true;

        var cantidadVendida = await GetCantidadVendidaAsync(rewardId, ct);
        return cantidadVendida < reward.CantidadMaxima.Value;
    }
}
