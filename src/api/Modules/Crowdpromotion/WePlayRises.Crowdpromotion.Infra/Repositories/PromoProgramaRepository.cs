using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Interfaces;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.Crowdpromotion.Infra.Context;

namespace WePlayRises.Crowdpromotion.Infra.Repositories;

public class PromoProgramaRepository : IPromoProgramaRepository
{
    private readonly CrowdpromotionContext _context;

    public PromoProgramaRepository(CrowdpromotionContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PromoPrograma?> GetByIdAsync(PromoProgramaId id, CancellationToken ct)
    {
        return await _context.Programas
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<PromoPrograma?> GetByIdWithTareasAsync(PromoProgramaId id, CancellationToken ct)
    {
        return await _context.Programas
            .Include(p => p.Tareas.Where(t => t.EsActivo))
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<PromoPrograma?> GetByIdWithFullDetailAsync(PromoProgramaId id, CancellationToken ct)
    {
        return await _context.Programas
            .Include(p => p.Tareas)
                .ThenInclude(t => t.TareasPromotor)
            .Include(p => p.Promotores)
                .ThenInclude(pp => pp.Promotor)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<(IReadOnlyList<PromoPrograma> Items, int TotalCount)> GetByArtistaIdPagedAsync(
        ArtistaId artistaId, bool? esActivo, int page, int pageSize, CancellationToken ct)
    {
        var query = _context.Programas
            .Where(x => x.ArtistaId == artistaId);

        if (esActivo.HasValue)
            query = query.Where(x => x.EsActivo == esActivo.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Include(p => p.Tareas.Where(t => t.EsActivo))
            .Include(p => p.Promotores)
            .AsNoTracking()
            .OrderByDescending(x => x.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<PromoProgramaId> AddAsync(PromoPrograma entity, CancellationToken ct)
    {
        await _context.Programas.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(PromoPrograma entity, CancellationToken ct)
    {
        _context.Programas.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsByCodigoTrackingAndArtistaAsync(
        string codigoTracking, ArtistaId artistaId, CancellationToken ct)
    {
        return await _context.Programas
            .AnyAsync(x => x.CodigoTrackingBase == codigoTracking && x.ArtistaId == artistaId, ct);
    }

    public async Task<bool> ExistsByCodigoTrackingAndArtistaExcludingIdAsync(
        string codigoTracking, ArtistaId artistaId, PromoProgramaId excludeId, CancellationToken ct)
    {
        return await _context.Programas
            .AnyAsync(x => x.CodigoTrackingBase == codigoTracking
                && x.ArtistaId == artistaId
                && x.Id != excludeId, ct);
    }

    public async Task<(IReadOnlyList<PromoPrograma> Items, int TotalCount)> GetActivosPagedAsync(
        string? artistaNombre, int? tipoPromoId, int page, int pageSize, PromotorId? promotorId, CancellationToken ct)
    {
        var query = _context.Programas
            .Where(x => x.EsActivo);

        if (tipoPromoId.HasValue)
            query = query.Where(x => x.TipoPromoId == tipoPromoId.Value);

        var totalCount = await query.CountAsync(ct);

        var itemsQuery = query
            .Include(p => p.Tareas.Where(t => t.EsActivo))
            .AsNoTracking()
            .OrderByDescending(x => x.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        if (promotorId.HasValue)
        {
            itemsQuery = itemsQuery
                .Include(p => p.Promotores.Where(pp => pp.PromotorId == promotorId.Value));
        }

        var items = await itemsQuery.ToListAsync(ct);

        return (items, totalCount);
    }
}
