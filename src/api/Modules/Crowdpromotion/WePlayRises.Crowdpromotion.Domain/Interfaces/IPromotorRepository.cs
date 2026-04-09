using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Domain.Interfaces;

public interface IPromotorRepository
{
    Task<Promotor?> GetByIdAsync(PromotorId id, CancellationToken ct);
    Task<Promotor?> GetByUserIdAsync(string userId, CancellationToken ct);
    Task<Promotor?> GetByUserIdWithWalletAsync(string userId, CancellationToken ct);
    Task<PromotorId> AddAsync(Promotor entity, CancellationToken ct);
    Task UpdateAsync(Promotor entity, CancellationToken ct);
    Task<bool> ExistsByUserIdAsync(string userId, CancellationToken ct);
    Task<IReadOnlyList<PromoProgramaPromotor>> GetActiveProgramasAsync(PromotorId promotorId, CancellationToken ct);
}
