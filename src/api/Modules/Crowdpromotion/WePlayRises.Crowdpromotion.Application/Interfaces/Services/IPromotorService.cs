using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Interfaces.Services;

public interface IPromotorService
{
    Task<Promotor?> GetByIdAsync(PromotorId id, CancellationToken ct);
    Task<Promotor?> GetByUserIdAsync(string userId, CancellationToken ct);
    Task<bool> TipoPromotorExistsAsync(int tipoPromotorId, CancellationToken ct);
    Task<PromotorId> CreateWithWalletAsync(Promotor promotor, CancellationToken ct);
    Task UpdateAsync(Promotor promotor, CancellationToken ct);
    Task<int> DesactivarWithProgramasAsync(PromotorId promotorId, CancellationToken ct);
    Task<int> GetProgramasActivosCountAsync(PromotorId promotorId, CancellationToken ct);
    Task<PromotorWallet?> GetWalletEurAsync(PromotorId promotorId, CancellationToken ct);
}
