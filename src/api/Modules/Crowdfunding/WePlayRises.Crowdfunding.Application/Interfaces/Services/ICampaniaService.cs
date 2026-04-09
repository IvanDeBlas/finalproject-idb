using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Interfaces.Services;

public interface ICampaniaService
{
    Task<CampaniaCrowdfunding?> GetByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct);
    Task<IReadOnlyList<CampaniaCrowdfunding>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<CampaniaCrowdfunding>> GetByArtistaIdAsync(ArtistaId artistaId, CancellationToken ct);
    Task<CampaniaCrowdfundingId> CreateAsync(CampaniaCrowdfunding campania, CancellationToken ct);
    Task UpdateAsync(CampaniaCrowdfunding campania, CancellationToken ct);
    Task<bool> ExistsAsync(CampaniaCrowdfundingId id, CancellationToken ct);
    Task<bool> ExistsByTituloAsync(string titulo, CancellationToken ct);
    Task<bool> ExistsByTituloExcludingIdAsync(string titulo, CampaniaCrowdfundingId excludeId, CancellationToken ct);
    Task<CampaniaCrowdfunding?> GetDetailByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct);
    Task<int> GetTotalBackersAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);
}
