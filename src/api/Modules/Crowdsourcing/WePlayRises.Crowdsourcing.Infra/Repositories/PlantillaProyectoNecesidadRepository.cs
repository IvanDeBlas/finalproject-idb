using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class PlantillaProyectoNecesidadRepository : IPlantillaProyectoNecesidadRepository
{
    private readonly CrowdsourcingContext _context;

    public PlantillaProyectoNecesidadRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PlantillaProyectoNecesidad?> GetByIdAsync(PlantillaProyectoNecesidadId id, CancellationToken ct)
    {
        return await _context.PlantillasProyectoNecesidades
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<PlantillaProyectoNecesidad>> GetByPlantillaIdAsync(PlantillaProyectoId plantillaId, CancellationToken ct)
    {
        return await _context.PlantillasProyectoNecesidades
            .AsNoTracking()
            .Where(x => x.PlantillaProyectoId == plantillaId)
            .OrderBy(x => x.Orden)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PlantillaProyectoNecesidad>> GetByIdsAsync(IEnumerable<PlantillaProyectoNecesidadId> ids, CancellationToken ct)
    {
        return await _context.PlantillasProyectoNecesidades
            .AsNoTracking()
            .Include(x => x.RolProfesional)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(ct);
    }

    public async Task<PlantillaProyectoNecesidadId> AddAsync(PlantillaProyectoNecesidad entity, CancellationToken ct)
    {
        await _context.PlantillasProyectoNecesidades.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(PlantillaProyectoNecesidad entity, CancellationToken ct)
    {
        _context.PlantillasProyectoNecesidades.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(PlantillaProyectoNecesidadId id, CancellationToken ct)
    {
        var entity = await _context.PlantillasProyectoNecesidades.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            _context.PlantillasProyectoNecesidades.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}
