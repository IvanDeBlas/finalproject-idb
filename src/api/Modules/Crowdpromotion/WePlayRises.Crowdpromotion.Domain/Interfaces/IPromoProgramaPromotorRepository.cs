using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Domain.Interfaces;

public interface IPromoProgramaPromotorRepository
{
    Task<PromoProgramaPromotor?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PromoProgramaPromotor?> GetByIdWithPromotorAsync(Guid id, CancellationToken ct);
    Task<PromoProgramaPromotor?> GetByPromotorAndProgramaAsync(PromotorId promotorId, PromoProgramaId programaId, CancellationToken ct);
    Task<(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)> GetPagedByProgramaAsync(
        PromoProgramaId programaId, string? estadoFiltro, int page, int pageSize, CancellationToken ct);
    Task<(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)> GetPagedByPromotorAsync(
        PromotorId promotorId, int page, int pageSize, CancellationToken ct);
    Task<bool> ExistsByCodigoReferidoAsync(string codigoReferido, CancellationToken ct);
    Task<PromoProgramaPromotor?> GetByCodigoReferidoActivoAsync(string codigoReferido, CancellationToken ct);
    Task<Guid> AddAsync(PromoProgramaPromotor entity, CancellationToken ct);
    Task UpdateAsync(PromoProgramaPromotor entity, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
