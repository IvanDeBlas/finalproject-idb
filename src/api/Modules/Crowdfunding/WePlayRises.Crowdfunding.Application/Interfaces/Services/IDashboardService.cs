using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<(decimal TotalRecaudado, int TotalBackers, int CampaniasActivas, int CampaniasCompletadas)>
        GetResumenAsync(ArtistaId artistaId, CancellationToken ct);

    Task<decimal> GetBackingPromedioAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);

    Task<string?> GetRewardMasPopularAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);

    Task<decimal> GetVelocidadDiariaAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);

    Task<decimal?> CalcularProyeccionFinalAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
}
