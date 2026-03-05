using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Interfaces.Services;

public interface IBackingService
{
    Task<PedidoCrowdfundingId> CreateBackingAsync(
        CampaniaCrowdfundingId campaniaId,
        CampaniaCrowdfundingRewardId? rewardId,
        decimal monto,
        string? userId,
        string? mensaje,
        bool esAnonimo,
        CancellationToken ct);
}
