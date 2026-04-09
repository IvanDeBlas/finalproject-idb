using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Domain.Interfaces;

public interface IPromotorWalletTransaccionRepository
{
    Task<(IReadOnlyList<PromotorWalletTransaccion> Items, int TotalCount)> GetPagedByWalletIdAsync(
        Guid walletId, bool? esCredito, int? estadoTransaccionId,
        DateTime? fechaDesde, DateTime? fechaHasta,
        int page, int pageSize, CancellationToken ct);

    Task AddAsync(PromotorWalletTransaccion entity, CancellationToken ct);

    Task<bool> HasPendienteByWalletIdAsync(Guid walletId, CancellationToken ct);
}
