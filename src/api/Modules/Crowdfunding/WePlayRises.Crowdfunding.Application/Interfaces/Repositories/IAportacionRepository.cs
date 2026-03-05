using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Interfaces.Repositories;

public interface IAportacionRepository
{
    Task<AportacionCrowdfundingId> AddAsync(AportacionCrowdfunding entity, CancellationToken ct);
    Task<AportacionCrowdfunding?> GetByPedidoIdAsync(PedidoCrowdfundingId pedidoId, CancellationToken ct);
}
