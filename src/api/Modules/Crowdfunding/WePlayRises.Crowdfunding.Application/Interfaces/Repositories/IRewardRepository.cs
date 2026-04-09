using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Interfaces.Repositories;

public interface IRewardRepository
{
    Task<CampaniaCrowdfundingReward?> GetByIdAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct);
    Task<IReadOnlyList<CampaniaCrowdfundingReward>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<CampaniaCrowdfundingReward>> GetByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
    Task<CampaniaCrowdfundingRewardId> AddAsync(CampaniaCrowdfundingReward entity, CancellationToken ct);
    Task UpdateAsync(CampaniaCrowdfundingReward entity, CancellationToken ct);
    Task<bool> ExistsByNombreInCampaniaAsync(string nombre, CampaniaCrowdfundingId campaniaId, CancellationToken ct);
    Task<bool> ExistsByNombreInCampaniaExcludingIdAsync(string nombre, CampaniaCrowdfundingId campaniaId, CampaniaCrowdfundingRewardId excludeId, CancellationToken ct);

    Task<CampaniaCrowdfundingReward?> GetByIdWithLineasAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct);
    Task<int> GetMaxOrdenAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
    Task UpdateBulkOrdenAsync(IEnumerable<(CampaniaCrowdfundingRewardId Id, int Orden)> updates, CancellationToken ct);
    Task<bool> HasBackingsAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct);
    Task<int> GetCantidadVendidaAsync(CampaniaCrowdfundingRewardId rewardId, CancellationToken ct);
}
