using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Domain.Interfaces;

public interface IPromoProgramaRepository
{
    Task<PromoPrograma?> GetByIdAsync(PromoProgramaId id, CancellationToken ct);
    Task<PromoPrograma?> GetByIdWithTareasAsync(PromoProgramaId id, CancellationToken ct);
    Task<PromoPrograma?> GetByIdWithFullDetailAsync(PromoProgramaId id, CancellationToken ct);
    Task<(IReadOnlyList<PromoPrograma> Items, int TotalCount)> GetByArtistaIdPagedAsync(
        ArtistaId artistaId, bool? esActivo, int page, int pageSize, CancellationToken ct);
    Task<PromoProgramaId> AddAsync(PromoPrograma entity, CancellationToken ct);
    Task UpdateAsync(PromoPrograma entity, CancellationToken ct);
    Task<bool> ExistsByCodigoTrackingAndArtistaAsync(string codigoTracking, ArtistaId artistaId, CancellationToken ct);
    Task<bool> ExistsByCodigoTrackingAndArtistaExcludingIdAsync(
        string codigoTracking, ArtistaId artistaId, PromoProgramaId excludeId, CancellationToken ct);
    Task<(IReadOnlyList<PromoPrograma> Items, int TotalCount)> GetActivosPagedAsync(
        string? artistaNombre, int? tipoPromoId, int page, int pageSize, PromotorId? promotorId, CancellationToken ct);
}
