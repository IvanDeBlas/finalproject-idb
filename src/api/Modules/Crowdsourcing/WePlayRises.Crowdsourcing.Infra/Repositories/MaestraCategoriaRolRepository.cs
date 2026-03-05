using Microsoft.EntityFrameworkCore;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class MaestraCategoriaRolRepository : IMaestraCategoriaRolRepository
{
    private readonly CrowdsourcingContext _context;

    public MaestraCategoriaRolRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<MaestraCategoriaRol?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.MaestrasCategoriaRol
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<MaestraCategoriaRol>> GetAllAsync(CancellationToken ct)
    {
        return await _context.MaestrasCategoriaRol
            .AsNoTracking()
            .OrderBy(x => x.Orden)
            .ToListAsync(ct);
    }
}
