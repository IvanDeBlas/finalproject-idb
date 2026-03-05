using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Domain.Interfaces;

public interface IPromoTareaPromotorRepository
{
    Task<PromoTareaPromotor?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PromoTareaPromotor?> GetByIdWithTareaAndProgramaAsync(Guid id, CancellationToken ct);
    Task<PromoTareaPromotor?> GetUltimoByTareaAndProgramaPromotorAsync(Guid tareaId, Guid programaPromotorId, CancellationToken ct);
    Task<int> CountCompletadosActivosAsync(Guid tareaId, Guid programaPromotorId, CancellationToken ct);
    Task<(IReadOnlyList<PromoTareaPromotor> Items, int TotalCount)> GetPendientesPagedAsync(
        PromoProgramaId programaId, int page, int pageSize, CancellationToken ct);
    Task<Guid> AddAsync(PromoTareaPromotor entity, CancellationToken ct);
    Task UpdateAsync(PromoTareaPromotor entity, CancellationToken ct);
}
