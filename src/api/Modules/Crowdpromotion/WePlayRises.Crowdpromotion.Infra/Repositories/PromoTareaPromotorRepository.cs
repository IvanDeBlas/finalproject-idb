using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Interfaces;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.Crowdpromotion.Infra.Context;

namespace WePlayRises.Crowdpromotion.Infra.Repositories;

public class PromoTareaPromotorRepository : IPromoTareaPromotorRepository
{
    private readonly CrowdpromotionContext _context;

    public PromoTareaPromotorRepository(CrowdpromotionContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PromoTareaPromotor?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.TareaPromotores
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<PromoTareaPromotor?> GetByIdWithTareaAndProgramaAsync(Guid id, CancellationToken ct)
    {
        return await _context.TareaPromotores
            .Include(x => x.Tarea)
            .Include(x => x.ProgramaPromotor)
                .ThenInclude(pp => pp.Promotor)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<PromoTareaPromotor?> GetUltimoByTareaAndProgramaPromotorAsync(
        Guid tareaId, Guid programaPromotorId, CancellationToken ct)
    {
        return await _context.TareaPromotores
            .Where(x => x.TareaId == tareaId && x.ProgramaPromotorId == programaPromotorId)
            .OrderByDescending(x => x.FechaCreacion)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<int> CountCompletadosActivosAsync(
        Guid tareaId, Guid programaPromotorId, CancellationToken ct)
    {
        return await _context.TareaPromotores
            .CountAsync(x => x.TareaId == tareaId
                && x.ProgramaPromotorId == programaPromotorId
                && (x.EstadoTareaId == 2 || x.EstadoTareaId == 3), ct);
    }

    public async Task<(IReadOnlyList<PromoTareaPromotor> Items, int TotalCount)> GetPendientesPagedAsync(
        PromoProgramaId programaId, int page, int pageSize, CancellationToken ct)
    {
        var query = _context.TareaPromotores
            .Where(x => x.EstadoTareaId == 2 && x.ProgramaPromotor.ProgramaId == programaId)
            .Include(x => x.Tarea)
            .Include(x => x.ProgramaPromotor)
                .ThenInclude(pp => pp.Promotor)
            .AsNoTracking();

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<Guid> AddAsync(PromoTareaPromotor entity, CancellationToken ct)
    {
        await _context.TareaPromotores.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(PromoTareaPromotor entity, CancellationToken ct)
    {
        _context.TareaPromotores.Update(entity);
        await _context.SaveChangesAsync(ct);
    }
}
