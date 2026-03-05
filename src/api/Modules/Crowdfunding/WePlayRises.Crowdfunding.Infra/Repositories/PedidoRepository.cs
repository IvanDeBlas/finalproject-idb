using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Domain.Model;
using WePlayRises.Crowdfunding.Infra.Context;

namespace WePlayRises.Crowdfunding.Infra.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly CrowdfundingContext _context;

    public PedidoRepository(CrowdfundingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PedidoCrowdfunding?> GetByIdAsync(PedidoCrowdfundingId id, CancellationToken ct)
    {
        return await _context.Pedidos
            .Include(p => p.Lineas)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<PedidoCrowdfunding>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PedidoCrowdfunding>> GetByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Where(p => p.CampaniaId == campaniaId)
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PedidoCrowdfunding>> GetByUserIdAsync(string userId, CancellationToken ct)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync(ct);
    }

    public async Task<PedidoCrowdfundingId> AddAsync(PedidoCrowdfunding entity, CancellationToken ct)
    {
        await _context.Pedidos.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(PedidoCrowdfunding entity, CancellationToken ct)
    {
        _context.Pedidos.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<PedidoCrowdfunding>> GetRecentByCampaniaIdAsync(
        CampaniaCrowdfundingId campaniaId,
        int limit,
        CancellationToken ct)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Lineas)
                .ThenInclude(l => l.Reward)
            .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3)
            .OrderByDescending(p => p.FechaCreacion)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<int> CountByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        return await _context.Pedidos
            .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3)
            .CountAsync(ct);
    }

    public async Task<(decimal Total, decimal Average, decimal Min, decimal Max)> GetStatsByCampaniaIdAsync(
        CampaniaCrowdfundingId campaniaId,
        CancellationToken ct)
    {
        var pedidos = _context.Pedidos
            .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3);

        var count = await pedidos.CountAsync(ct);
        if (count == 0)
            return (0, 0, 0, 0);

        var total = await pedidos.SumAsync(p => p.ImporteTotal, ct);
        var avg = await pedidos.AverageAsync(p => p.ImporteTotal, ct);
        var min = await pedidos.MinAsync(p => p.ImporteTotal, ct);
        var max = await pedidos.MaxAsync(p => p.ImporteTotal, ct);

        return (total, avg, min, max);
    }

    public async Task<IReadOnlyList<PedidoCrowdfunding>> GetByCampaniaIdPaginatedAsync(
        CampaniaCrowdfundingId campaniaId,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Lineas)
                .ThenInclude(l => l.Reward)
            .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3)
            .OrderByDescending(p => p.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<Dictionary<Guid, int>> GetRewardStatsByCampaniaIdAsync(
        CampaniaCrowdfundingId campaniaId,
        CancellationToken ct)
    {
        var lineas = await _context.PedidoLineas
            .AsNoTracking()
            .Where(l => l.PedidoCrowdfunding.CampaniaId == campaniaId
                     && l.PedidoCrowdfunding.EstadoPedidoId == 3)
            .Select(l => new { RewardId = l.RewardId, Cantidad = l.Cantidad })
            .ToListAsync(ct);

        return lineas
            .GroupBy(l => l.RewardId.Value)
            .ToDictionary(g => g.Key, g => g.Sum(l => l.Cantidad));
    }

    public async Task<PedidoCrowdfunding?> GetLastByCampaniaIdAsync(
        CampaniaCrowdfundingId campaniaId,
        CancellationToken ct)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3)
            .OrderByDescending(p => p.FechaCreacion)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<(DateTime Date, int Count, decimal Total)>> GetProgressByDayAsync(
        CampaniaCrowdfundingId campaniaId,
        CancellationToken ct)
    {
        var results = await _context.Pedidos
            .AsNoTracking()
            .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3)
            .GroupBy(p => p.FechaCreacion.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count(),
                Total = g.Sum(p => p.ImporteTotal)
            })
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

        return results
            .Select(x => (x.Date, x.Count, x.Total))
            .ToList();
    }
}
