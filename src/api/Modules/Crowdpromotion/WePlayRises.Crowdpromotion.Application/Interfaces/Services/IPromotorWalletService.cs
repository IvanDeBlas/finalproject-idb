using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Interfaces.Services;

public interface IPromotorWalletService
{
    Task<PromotorWallet?> GetWalletEurByPromotorIdAsync(PromotorId promotorId, CancellationToken ct);

    Task<Promotor?> GetPromotorByUserIdAsync(string userId, CancellationToken ct);

    Task<(PromotorWallet? Wallet, string? MonedaNombre)> GetWalletConMonedaAsync(
        PromotorId promotorId, CancellationToken ct);

    Task<(IReadOnlyList<PromotorWalletTransaccion> Items, int TotalCount)> GetTransaccionesPagedAsync(
        Guid walletId, bool? esCredito, int? estadoTransaccionId,
        DateTime? fechaDesde, DateTime? fechaHasta,
        int page, int pageSize, CancellationToken ct);

    Task<(SolicitarCobroResult? Result, CobroError Error)> SolicitarCobroAsync(
        PromotorId promotorId, decimal importe, string? descripcion, CancellationToken ct);
}

public enum CobroError
{
    None = 0,
    WalletNoEncontrado = 1,
    SaldoBajoMinimo = 2,
    SaldoInsuficiente = 3,
    CobroConcurrente = 4
}

public record SolicitarCobroResult(
    Guid TransaccionId,
    decimal Importe,
    string MonedaNombre,
    decimal SaldoRestante,
    DateTime FechaCreacion);
