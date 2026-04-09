using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Domain.Model;
using WePlayRises.Crowdfunding.Infra.Context;

namespace WePlayRises.Crowdfunding.Infra.Repositories;

public class CampaniaRepository : ICampaniaRepository
{
    private readonly CrowdfundingContext _context;

    public CampaniaRepository(CrowdfundingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<CampaniaCrowdfunding?> GetByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct)
    {
        return await _context.Campanias
            .Include(c => c.Rewards)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<CampaniaCrowdfunding>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Campanias
            .AsNoTracking()
            .OrderByDescending(c => c.FechaCreacion)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CampaniaCrowdfunding>> GetByArtistaIdAsync(ArtistaId artistaId, CancellationToken ct)
    {
        return await _context.Campanias
            .AsNoTracking()
            .Where(c => c.ArtistaId == artistaId)
            .OrderByDescending(c => c.FechaCreacion)
            .ToListAsync(ct);
    }

    public async Task<CampaniaCrowdfundingId> AddAsync(CampaniaCrowdfunding entity, CancellationToken ct)
    {
        await _context.Campanias.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(CampaniaCrowdfunding entity, CancellationToken ct)
    {
        _context.Campanias.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsByTituloAsync(string titulo, CancellationToken ct)
    {
        return await _context.Campanias
            .AnyAsync(c => c.Titulo == titulo && !c.Borrado, ct);
    }

    public async Task<bool> ExistsByTituloExcludingIdAsync(string titulo, CampaniaCrowdfundingId excludeId, CancellationToken ct)
    {
        return await _context.Campanias
            .AnyAsync(c => c.Titulo == titulo && c.Id != excludeId && !c.Borrado, ct);
    }

    public async Task<CampaniaCrowdfunding?> GetDetailByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct)
    {
        return await _context.Campanias
            .AsNoTracking()
            .Include(c => c.Rewards.Where(r => r.EsActivo).OrderBy(r => r.Orden))
            .Include(c => c.Pedidos
                .Where(p => p.EstadoPedidoId == 3)
                .OrderByDescending(p => p.FechaCreacion)
                .Take(10))
                .ThenInclude(p => p.Lineas)
                    .ThenInclude(l => l.Reward)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<int> CountBackersByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        return await _context.Pedidos
            .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == 3)
            .CountAsync(ct);
    }

    public async Task<IReadOnlyList<CampaniaCrowdfunding>> GetMisCampaniasPaginatedAsync(
        ArtistaId artistaId,
        int? estadoCampaniaId,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query = _context.Campanias
            .AsNoTracking()
            .Where(c => c.ArtistaId == artistaId && !c.Borrado);

        if (estadoCampaniaId.HasValue)
        {
            query = query.Where(c => c.EstadoCampaniaId == estadoCampaniaId.Value);
        }

        return await query
            .OrderByDescending(c => c.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountMisCampaniasAsync(
        ArtistaId artistaId,
        int? estadoCampaniaId,
        CancellationToken ct)
    {
        var query = _context.Campanias
            .Where(c => c.ArtistaId == artistaId && !c.Borrado);

        if (estadoCampaniaId.HasValue)
        {
            query = query.Where(c => c.EstadoCampaniaId == estadoCampaniaId.Value);
        }

        return await query.CountAsync(ct);
    }
}
