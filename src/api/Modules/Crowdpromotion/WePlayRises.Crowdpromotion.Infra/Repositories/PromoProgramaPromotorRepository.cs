using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Interfaces;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.Crowdpromotion.Infra.Context;

namespace WePlayRises.Crowdpromotion.Infra.Repositories;

public class PromoProgramaPromotorRepository : IPromoProgramaPromotorRepository
{
    private readonly CrowdpromotionContext _context;

    public PromoProgramaPromotorRepository(CrowdpromotionContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PromoProgramaPromotor?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.ProgramaPromotores
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<PromoProgramaPromotor?> GetByIdWithPromotorAsync(Guid id, CancellationToken ct)
    {
        return await _context.ProgramaPromotores
            .Include(pp => pp.Promotor)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<PromoProgramaPromotor?> GetByPromotorAndProgramaAsync(
        PromotorId promotorId, PromoProgramaId programaId, CancellationToken ct)
    {
        return await _context.ProgramaPromotores
            .FirstOrDefaultAsync(x => x.PromotorId == promotorId && x.ProgramaId == programaId, ct);
    }

    public async Task<(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)> GetPagedByProgramaAsync(
        PromoProgramaId programaId, string? estadoFiltro, int page, int pageSize, CancellationToken ct)
    {
        var query = _context.ProgramaPromotores
            .Where(x => x.ProgramaId == programaId);

        query = estadoFiltro switch
        {
            "Pendiente" => query.Where(x => !x.EsBloqueado && x.FechaBaja == null && !x.EsAprobado),
            "Aprobado" => query.Where(x => x.EsAprobado && x.FechaBaja == null),
            "Bloqueado" => query.Where(x => x.EsBloqueado),
            "DadoDeBaja" => query.Where(x => x.FechaBaja != null),
            _ => query
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Include(pp => pp.Promotor)
            .AsNoTracking()
            .OrderByDescending(x => x.FechaInscripcion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)> GetPagedByPromotorAsync(
        PromotorId promotorId, int page, int pageSize, CancellationToken ct)
    {
        var query = _context.ProgramaPromotores
            .Where(x => x.PromotorId == promotorId);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Include(pp => pp.Programa)
                .ThenInclude(p => p.Tareas.Where(t => t.EsActivo))
            .AsNoTracking()
            .OrderByDescending(x => x.FechaInscripcion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<bool> ExistsByCodigoReferidoAsync(string codigoReferido, CancellationToken ct)
    {
        return await _context.ProgramaPromotores
            .AnyAsync(x => x.CodigoReferido == codigoReferido, ct);
    }

    public async Task<Guid> AddAsync(PromoProgramaPromotor entity, CancellationToken ct)
    {
        await _context.ProgramaPromotores.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(PromoProgramaPromotor entity, CancellationToken ct)
    {
        _context.ProgramaPromotores.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await _context.ProgramaPromotores.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            _context.ProgramaPromotores.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<PromoProgramaPromotor?> GetByCodigoReferidoActivoAsync(string codigoReferido, CancellationToken ct)
    {
        return await _context.ProgramaPromotores
            .AsNoTracking()
            .Include(ppp => ppp.Promotor)
            .Include(ppp => ppp.Programa)
            .FirstOrDefaultAsync(ppp =>
                ppp.CodigoReferido == codigoReferido
                && ppp.EsAprobado
                && !ppp.EsBloqueado
                && ppp.FechaBaja == null, ct);
    }
}
