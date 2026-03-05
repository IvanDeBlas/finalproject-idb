using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Domain.Model;
using WePlayRises.Crowdfunding.Infra.Context;

namespace WePlayRises.Crowdfunding.Infra.Repositories;

public class AportacionRepository : IAportacionRepository
{
    private readonly CrowdfundingContext _context;

    public AportacionRepository(CrowdfundingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<AportacionCrowdfundingId> AddAsync(AportacionCrowdfunding entity, CancellationToken ct)
    {
        await _context.Aportaciones.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<AportacionCrowdfunding?> GetByPedidoIdAsync(PedidoCrowdfundingId pedidoId, CancellationToken ct)
    {
        return await _context.Aportaciones
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.PedidoCrowdfundingId == pedidoId, ct);
    }
}
