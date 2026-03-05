using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class AcuerdoCrowdsourcingRepository : IAcuerdoCrowdsourcingRepository
{
    private readonly CrowdsourcingContext _context;

    public AcuerdoCrowdsourcingRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<AcuerdoCrowdsourcing?> GetByIdAsync(AcuerdoCrowdsourcingId id, CancellationToken ct)
    {
        return await _context.Acuerdos
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<AcuerdoCrowdsourcing?> GetByIdWithDetailsAsync(AcuerdoCrowdsourcingId id, CancellationToken ct)
    {
        return await _context.Acuerdos
            .AsNoTracking()
            .Include(a => a.Milestones.OrderBy(m => m.Orden))
                .ThenInclude(m => m.Entregables)
            .Include(a => a.Entregables)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<AcuerdoCrowdsourcingId> AddAsync(AcuerdoCrowdsourcing entity, CancellationToken ct)
    {
        await _context.Acuerdos.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(AcuerdoCrowdsourcing entity, CancellationToken ct)
    {
        _context.Acuerdos.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExisteAcuerdoActivoParaNecesidadAsync(NecesidadCrowdsourcingId necesidadId, CancellationToken ct)
    {
        return await _context.Acuerdos
            .AnyAsync(a =>
                a.NecesidadId == necesidadId
                && a.EstadoAcuerdoId == EstadoAcuerdoConstants.Activo, ct);
    }
}
