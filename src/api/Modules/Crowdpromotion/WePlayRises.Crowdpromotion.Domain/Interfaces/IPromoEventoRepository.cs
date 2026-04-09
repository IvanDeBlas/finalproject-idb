using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Domain.Interfaces;

// Raw result types for aggregate queries
public record PromoEventoMetricasRaw(
    int TotalClicks,
    int TotalPageViews,
    int TotalSignups,
    int TotalConversiones,
    decimal ValorTotalGenerado);

public record PromoEventoRankingRaw(
    Guid PromoProgramaPromotorId,
    int Clicks,
    int PageViews,
    int Signups,
    int Conversiones,
    decimal ValorGenerado);

public record PromoEventoPorDiaRaw(
    DateOnly Fecha,
    int Clicks,
    int PageViews,
    int Signups,
    int Conversiones);

public record PromoEventoMetricasPromotorRaw(
    int MisClicks,
    int MisPageViews,
    int MisSignups,
    int MisConversiones,
    decimal MiValorGenerado);

public interface IPromoEventoRepository
{
    Task<Guid> AddAsync(PromoEvento entity, CancellationToken ct);

    Task<PromoEventoMetricasRaw> GetMetricasProgramaAsync(
        PromoProgramaId programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct);

    Task<IReadOnlyList<PromoEventoRankingRaw>> GetRankingPromotoresAsync(
        PromoProgramaId programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct);

    Task<IReadOnlyList<PromoEventoPorDiaRaw>> GetEventosPorDiaAsync(
        PromoProgramaId programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct);

    Task<PromoEventoMetricasPromotorRaw> GetMetricasPromotorAsync(
        PromotorId promotorId, Guid? programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct);

    Task<IReadOnlyList<PromoEvento>> GetEventosRecientesByPromotorAsync(
        PromotorId promotorId, Guid? programaId, int maxItems, CancellationToken ct);
}
