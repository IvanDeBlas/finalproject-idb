using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Interfaces.Services;

public interface IPedidoService
{
    Task<PedidoCrowdfunding?> GetByIdAsync(PedidoCrowdfundingId id, CancellationToken ct);
    Task<IReadOnlyList<PedidoCrowdfunding>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<PedidoCrowdfunding>> GetByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
    Task<IReadOnlyList<PedidoCrowdfunding>> GetByUserIdAsync(string userId, CancellationToken ct);
    Task<PedidoCrowdfundingId> CreateAsync(PedidoCrowdfunding pedido, CancellationToken ct);
    Task UpdateAsync(PedidoCrowdfunding pedido, CancellationToken ct);
    Task<bool> ExistsAsync(PedidoCrowdfundingId id, CancellationToken ct);
    Task<IReadOnlyList<PedidoCrowdfunding>> GetRecentByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, int limit, CancellationToken ct);
    Task<int> CountByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
    Task<(decimal Total, decimal Average, decimal Min, decimal Max)> GetStatsByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
}
