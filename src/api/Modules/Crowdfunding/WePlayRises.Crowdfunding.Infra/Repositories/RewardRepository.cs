using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Domain.Model;
using WePlayRises.Crowdfunding.Infra.Context;

namespace WePlayRises.Crowdfunding.Infra.Repositories;

public class RewardRepository : IRewardRepository
{
    private readonly CrowdfundingContext _context;

    public RewardRepository(CrowdfundingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<CampaniaCrowdfundingReward?> GetByIdAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct)
    {
        return await _context.Rewards
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<CampaniaCrowdfundingReward>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Rewards
            .AsNoTracking()
            .OrderBy(r => r.Orden)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CampaniaCrowdfundingReward>> GetByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        return await _context.Rewards
            .AsNoTracking()
            .Where(r => r.CampaniaId == campaniaId)
            .OrderBy(r => r.Orden)
            .ToListAsync(ct);
    }

    public async Task<CampaniaCrowdfundingRewardId> AddAsync(CampaniaCrowdfundingReward entity, CancellationToken ct)
    {
        await _context.Rewards.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(CampaniaCrowdfundingReward entity, CancellationToken ct)
    {
        _context.Rewards.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsByNombreInCampaniaAsync(string nombre, CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        return await _context.Rewards
            .AnyAsync(r => r.Nombre == nombre && r.CampaniaId == campaniaId && r.EsActivo, ct);
    }

    public async Task<bool> ExistsByNombreInCampaniaExcludingIdAsync(string nombre, CampaniaCrowdfundingId campaniaId, CampaniaCrowdfundingRewardId excludeId, CancellationToken ct)
    {
        return await _context.Rewards
            .AnyAsync(r => r.Nombre == nombre && r.CampaniaId == campaniaId && r.Id != excludeId && r.EsActivo, ct);
    }

    public async Task<CampaniaCrowdfundingReward?> GetByIdWithLineasAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct)
    {
        return await _context.Rewards
            .Include(r => r.Lineas)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<int> GetMaxOrdenAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var maxOrden = await _context.Rewards
            .Where(r => r.CampaniaId == campaniaId)
            .MaxAsync(r => (int?)r.Orden, ct);

        return maxOrden ?? 0;
    }

    public async Task UpdateBulkOrdenAsync(IEnumerable<(CampaniaCrowdfundingRewardId Id, int Orden)> updates, CancellationToken ct)
    {
        foreach (var (id, orden) in updates)
        {
            var reward = await _context.Rewards.FindAsync(new object[] { id }, ct);
            if (reward != null)
            {
                reward.Orden = orden;
                reward.FechaActualizacion = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> HasBackingsAsync(CampaniaCrowdfundingRewardId id, CancellationToken ct)
    {
        return await _context.PedidoLineas
            .AnyAsync(l => l.RewardId == id, ct);
    }

    public async Task<int> GetCantidadVendidaAsync(CampaniaCrowdfundingRewardId rewardId, CancellationToken ct)
    {
        return await _context.PedidoLineas
            .Where(l => l.RewardId == rewardId &&
                        l.PedidoCrowdfunding.EstadoPedidoId == 3)
            .SumAsync(l => l.Cantidad, ct);
    }
}
