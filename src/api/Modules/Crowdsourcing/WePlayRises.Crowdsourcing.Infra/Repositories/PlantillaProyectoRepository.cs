using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class PlantillaProyectoRepository : IPlantillaProyectoRepository
{
    private readonly CrowdsourcingContext _context;

    public PlantillaProyectoRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PlantillaProyecto?> GetByIdAsync(PlantillaProyectoId id, CancellationToken ct)
    {
        return await _context.PlantillasProyecto
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<PlantillaProyecto?> GetByIdWithNecesidadesAsync(PlantillaProyectoId id, CancellationToken ct)
    {
        return await _context.PlantillasProyecto
            .AsNoTracking()
            .Include(x => x.Necesidades)
                .ThenInclude(n => n.RolProfesional)
                    .ThenInclude(r => r.CategoriaRol)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<PlantillaProyecto>> GetAllActivosAsync(CancellationToken ct)
    {
        return await _context.PlantillasProyecto
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Orden)
            .ToListAsync(ct);
    }

    public async Task<PlantillaProyectoId> AddAsync(PlantillaProyecto entity, CancellationToken ct)
    {
        await _context.PlantillasProyecto.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(PlantillaProyecto entity, CancellationToken ct)
    {
        _context.PlantillasProyecto.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(PlantillaProyectoId id, CancellationToken ct)
    {
        var entity = await _context.PlantillasProyecto.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            entity.Activo = false;
            await _context.SaveChangesAsync(ct);
        }
    }
}
