using Microsoft.EntityFrameworkCore;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class MaestraRolProfesionalRepository : IMaestraRolProfesionalRepository
{
    private readonly CrowdsourcingContext _context;

    public MaestraRolProfesionalRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<MaestraRolProfesional?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.MaestrasRolProfesional
            .AsNoTracking()
            .Include(x => x.CategoriaRol)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<MaestraRolProfesional>> GetAllActivosAsync(CancellationToken ct)
    {
        return await _context.MaestrasRolProfesional
            .AsNoTracking()
            .Include(x => x.CategoriaRol)
            .Where(x => x.Activo)
            .OrderBy(x => x.CategoriaRol.Orden)
                .ThenBy(x => x.Nombre)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<MaestraRolProfesional>> GetByCategoriaIdAsync(int categoriaId, CancellationToken ct)
    {
        return await _context.MaestrasRolProfesional
            .AsNoTracking()
            .Where(x => x.CategoriaRolId == categoriaId && x.Activo)
            .OrderBy(x => x.Nombre)
            .ToListAsync(ct);
    }
}
