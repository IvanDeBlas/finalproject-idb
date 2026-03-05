using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Interfaces;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.Crowdpromotion.Infra.Context;

namespace WePlayRises.Crowdpromotion.Infra.Repositories;

public class PromotorWalletRepository : IPromotorWalletRepository
{
    private readonly CrowdpromotionContext _context;

    public PromotorWalletRepository(CrowdpromotionContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PromotorWallet?> GetByPromotorIdAndMonedaAsync(PromotorId promotorId, int monedaId, CancellationToken ct)
    {
        return await _context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PromotorId == promotorId && x.MonedaId == monedaId, ct);
    }

    public async Task<PromotorWallet?> GetByPromotorIdAndMonedaForUpdateAsync(
        PromotorId promotorId, int monedaId, CancellationToken ct)
    {
        return await _context.Wallets
            .FirstOrDefaultAsync(x => x.PromotorId == promotorId && x.MonedaId == monedaId, ct);
    }

    public async Task<Guid> AddAsync(PromotorWallet entity, CancellationToken ct)
    {
        await _context.Wallets.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public Task UpdateSaldoAsync(PromotorWallet entity, CancellationToken ct)
    {
        _context.Wallets.Update(entity);
        return Task.CompletedTask;
    }
}
