using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Interfaces;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.Crowdpromotion.Infra.Context;

namespace WePlayRises.Crowdpromotion.Infra.Repositories;

public class PromoTareaRepository : IPromoTareaRepository
{
    private readonly CrowdpromotionContext _context;

    public PromoTareaRepository(CrowdpromotionContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PromoTarea?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Tareas
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<PromoTarea?> GetByIdAndProgramaAsync(Guid tareaId, PromoProgramaId programaId, CancellationToken ct)
    {
        return await _context.Tareas
            .FirstOrDefaultAsync(x => x.Id == tareaId && x.ProgramaId == programaId, ct);
    }

    public async Task<IReadOnlyList<PromoTarea>> GetTareasByProgramaAsync(PromoProgramaId programaId, CancellationToken ct)
    {
        return await _context.Tareas
            .AsNoTracking()
            .Where(x => x.ProgramaId == programaId && x.EsActivo)
            .OrderBy(x => x.Orden)
            .ToListAsync(ct);
    }
}
