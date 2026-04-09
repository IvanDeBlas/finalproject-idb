using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Interfaces;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.Crowdpromotion.Infra.Context;

namespace WePlayRises.Crowdpromotion.Infra.Repositories;

public class PromotorRepository : IPromotorRepository
{
    private readonly CrowdpromotionContext _context;

    public PromotorRepository(CrowdpromotionContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Promotor?> GetByIdAsync(PromotorId id, CancellationToken ct)
    {
        return await _context.Promotores
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Promotor?> GetByUserIdAsync(string userId, CancellationToken ct)
    {
        return await _context.Promotores
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);
    }

    public async Task<Promotor?> GetByUserIdWithWalletAsync(string userId, CancellationToken ct)
    {
        return await _context.Promotores
            .Include(p => p.Wallets)
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);
    }

    public async Task<PromotorId> AddAsync(Promotor entity, CancellationToken ct)
    {
        await _context.Promotores.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(Promotor entity, CancellationToken ct)
    {
        _context.Promotores.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsByUserIdAsync(string userId, CancellationToken ct)
    {
        return await _context.Promotores
            .AnyAsync(x => x.UserId == userId, ct);
    }

    public async Task<IReadOnlyList<PromoProgramaPromotor>> GetActiveProgramasAsync(PromotorId promotorId, CancellationToken ct)
    {
        return await _context.ProgramaPromotores
            .Where(x => x.PromotorId == promotorId && x.EsActivo)
            .ToListAsync(ct);
    }
}
