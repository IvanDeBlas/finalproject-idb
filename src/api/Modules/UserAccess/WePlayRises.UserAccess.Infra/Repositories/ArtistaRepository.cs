using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.UserAccess.Domain.Interfaces;
using WePlayRises.UserAccess.Domain.Model;
using WePlayRises.UserAccess.Infra.Context;

namespace WePlayRises.UserAccess.Infra.Repositories;

public class ArtistaRepository : IArtistaRepository
{
    private readonly UserAccessContext _context;

    public ArtistaRepository(UserAccessContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Artista?> GetByIdAsync(ArtistaId id, CancellationToken ct)
    {
        return await _context.Artistas
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Artista?> GetByUserIdAsync(string userId, CancellationToken ct)
    {
        return await _context.Artistas
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserIdPropietario == userId, ct);
    }

    public async Task<bool> ExistsForUserAsync(string userId, CancellationToken ct)
    {
        return await _context.Artistas
            .AnyAsync(x => x.UserIdPropietario == userId, ct);
    }

    public async Task<ArtistaId> AddAsync(Artista entity, CancellationToken ct)
    {
        await _context.Artistas.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(Artista entity, CancellationToken ct)
    {
        _context.Artistas.Update(entity);
        await _context.SaveChangesAsync(ct);
    }
}
