using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Interfaces.Services;

public record RegistrarConversionResult(
    Guid EventoId,
    decimal ComisionCalculada,
    Guid? WalletTransaccionId,
    bool ComisionAcreditada,
    string? MonedaNombre);

public interface IPromoEventoService
{
    Task<PromoProgramaPromotor?> ResolverPromotorPorCodigoReferidoAsync(string codigoReferido, CancellationToken ct);
    Task<Guid> RegistrarEventoAsync(PromoEvento evento, CancellationToken ct);
    Task<RegistrarConversionResult> RegistrarConversionAsync(PromoEvento evento, CancellationToken ct);
    Task<ProgramaMetricasResponseDto> GetProgramaMetricasAsync(Guid programaId, DateOnly fechaDesde, DateOnly fechaHasta, CancellationToken ct);
    Task<PromotorMetricasResponseDto> GetPromotorMetricasAsync(PromotorId promotorId, Guid? programaId, DateOnly fechaDesde, DateOnly fechaHasta, CancellationToken ct);
    Task<Promotor?> GetPromotorByUserIdAsync(string userId, CancellationToken ct);
    Task<PromoPrograma?> GetProgramaByIdAsync(Guid programaId, CancellationToken ct);
}
