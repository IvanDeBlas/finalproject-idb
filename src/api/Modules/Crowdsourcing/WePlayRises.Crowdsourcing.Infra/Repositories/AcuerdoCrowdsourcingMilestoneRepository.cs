using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class AcuerdoCrowdsourcingMilestoneRepository : IAcuerdoCrowdsourcingMilestoneRepository
{
    private readonly CrowdsourcingContext _context;

    public AcuerdoCrowdsourcingMilestoneRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<AcuerdoCrowdsourcingMilestone?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Milestones
            .FirstOrDefaultAsync(m => m.Id == id, ct);
    }

    public async Task<IReadOnlyList<AcuerdoCrowdsourcingMilestone>> GetByAcuerdoIdAsync(
        AcuerdoCrowdsourcingId acuerdoId,
        CancellationToken ct)
    {
        return await _context.Milestones
            .AsNoTracking()
            .Where(m => m.AcuerdoId == acuerdoId)
            .OrderBy(m => m.Orden)
            .ToListAsync(ct);
    }

    public async Task<decimal> GetSumaImportesByAcuerdoIdAsync(AcuerdoCrowdsourcingId acuerdoId, CancellationToken ct)
    {
        return await _context.Milestones
            .Where(m => m.AcuerdoId == acuerdoId)
            .SumAsync(m => m.ImporteParcial, ct);
    }

    public async Task<decimal> GetSumaImportesExcluyendoAsync(
        AcuerdoCrowdsourcingId acuerdoId,
        Guid excludeMilestoneId,
        CancellationToken ct)
    {
        return await _context.Milestones
            .Where(m => m.AcuerdoId == acuerdoId && m.Id != excludeMilestoneId)
            .SumAsync(m => m.ImporteParcial, ct);
    }

    public async Task<int> GetMaxOrdenAsync(AcuerdoCrowdsourcingId acuerdoId, CancellationToken ct)
    {
        var maxOrden = await _context.Milestones
            .Where(m => m.AcuerdoId == acuerdoId)
            .MaxAsync(m => (int?)m.Orden, ct);

        return maxOrden ?? 0;
    }

    public async Task<bool> TieneEntregablesAsync(Guid milestoneId, CancellationToken ct)
    {
        return await _context.Entregables
            .AnyAsync(e => e.MilestoneId == milestoneId, ct);
    }

    public async Task<Guid> AddAsync(AcuerdoCrowdsourcingMilestone entity, CancellationToken ct)
    {
        await _context.Milestones.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(AcuerdoCrowdsourcingMilestone entity, CancellationToken ct)
    {
        _context.Milestones.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await _context.Milestones.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            _context.Milestones.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}
