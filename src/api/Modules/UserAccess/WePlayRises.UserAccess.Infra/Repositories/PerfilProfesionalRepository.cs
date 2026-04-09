using Microsoft.EntityFrameworkCore;
using WePlayRises.UserAccess.Domain.Interfaces;
using WePlayRises.UserAccess.Domain.Model;
using WePlayRises.UserAccess.Infra.Context;

namespace WePlayRises.UserAccess.Infra.Repositories;

public class PerfilProfesionalRepository : IPerfilProfesionalRepository
{
    private readonly UserAccessContext _context;

    public PerfilProfesionalRepository(UserAccessContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PerfilProfesional?> GetByUserIdAsync(string userId, CancellationToken ct)
    {
        return await _context.PerfilesProfesionales
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);
    }

    public async Task<bool> ExistsByUserIdAsync(string userId, CancellationToken ct)
    {
        return await _context.PerfilesProfesionales
            .AnyAsync(p => p.UserId == userId, ct);
    }
}
