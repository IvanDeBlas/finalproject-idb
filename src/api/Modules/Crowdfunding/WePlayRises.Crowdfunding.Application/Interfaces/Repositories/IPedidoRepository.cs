using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Interfaces.Repositories;

public interface IPedidoRepository
{
    Task<PedidoCrowdfunding?> GetByIdAsync(PedidoCrowdfundingId id, CancellationToken ct);
    Task<IReadOnlyList<PedidoCrowdfunding>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<PedidoCrowdfunding>> GetByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
    Task<IReadOnlyList<PedidoCrowdfunding>> GetByUserIdAsync(string userId, CancellationToken ct);
    Task<PedidoCrowdfundingId> AddAsync(PedidoCrowdfunding entity, CancellationToken ct);
    Task UpdateAsync(PedidoCrowdfunding entity, CancellationToken ct);
    Task<IReadOnlyList<PedidoCrowdfunding>> GetRecentByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, int limit, CancellationToken ct);
    Task<int> CountByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
    Task<(decimal Total, decimal Average, decimal Min, decimal Max)> GetStatsByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);

    // Dashboard methods
    Task<IReadOnlyList<PedidoCrowdfunding>> GetByCampaniaIdPaginatedAsync(
        CampaniaCrowdfundingId campaniaId,
        int page,
        int pageSize,
        CancellationToken ct);

    Task<Dictionary<Guid, int>> GetRewardStatsByCampaniaIdAsync(
        CampaniaCrowdfundingId campaniaId,
        CancellationToken ct);

    Task<PedidoCrowdfunding?> GetLastByCampaniaIdAsync(
        CampaniaCrowdfundingId campaniaId,
        CancellationToken ct);

    Task<IReadOnlyList<(DateTime Date, int Count, decimal Total)>> GetProgressByDayAsync(
        CampaniaCrowdfundingId campaniaId,
        CancellationToken ct);
}
