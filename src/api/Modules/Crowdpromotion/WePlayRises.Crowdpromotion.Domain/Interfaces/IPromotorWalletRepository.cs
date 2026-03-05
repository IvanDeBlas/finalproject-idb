using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Domain.Interfaces;

public interface IPromotorWalletRepository
{
    Task<PromotorWallet?> GetByPromotorIdAndMonedaAsync(PromotorId promotorId, int monedaId, CancellationToken ct);
    Task<PromotorWallet?> GetByPromotorIdAndMonedaForUpdateAsync(PromotorId promotorId, int monedaId, CancellationToken ct);
    Task<Guid> AddAsync(PromotorWallet entity, CancellationToken ct);
    Task UpdateSaldoAsync(PromotorWallet entity, CancellationToken ct);
}
