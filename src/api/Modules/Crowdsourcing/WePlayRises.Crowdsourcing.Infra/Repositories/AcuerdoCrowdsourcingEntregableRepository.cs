using Microsoft.EntityFrameworkCore;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class AcuerdoCrowdsourcingEntregableRepository : IAcuerdoCrowdsourcingEntregableRepository
{
    private readonly CrowdsourcingContext _context;

    public AcuerdoCrowdsourcingEntregableRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<AcuerdoCrowdsourcingEntregable?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Entregables
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<AcuerdoCrowdsourcingEntregable?> GetByIdWithAcuerdoAsync(Guid id, CancellationToken ct)
    {
        return await _context.Entregables
            .Include(e => e.Acuerdo)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<bool> TodosAprobadosEnMilestoneAsync(Guid milestoneId, CancellationToken ct)
    {
        var entregables = await _context.Entregables
            .Where(e => e.MilestoneId == milestoneId)
            .ToListAsync(ct);

        if (entregables.Count == 0)
        {
            return false;
        }

        return entregables.All(e => e.EstadoEntregableId == Domain.Constants.EstadoEntregableConstants.Aprobado);
    }

    public async Task<Guid> AddAsync(AcuerdoCrowdsourcingEntregable entity, CancellationToken ct)
    {
        await _context.Entregables.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(AcuerdoCrowdsourcingEntregable entity, CancellationToken ct)
    {
        _context.Entregables.Update(entity);
        await _context.SaveChangesAsync(ct);
    }
}
