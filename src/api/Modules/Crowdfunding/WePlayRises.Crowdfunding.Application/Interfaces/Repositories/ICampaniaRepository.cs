using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Interfaces.Repositories;

public interface ICampaniaRepository
{
    Task<CampaniaCrowdfunding?> GetByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct);
    Task<IReadOnlyList<CampaniaCrowdfunding>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<CampaniaCrowdfunding>> GetByArtistaIdAsync(ArtistaId artistaId, CancellationToken ct);
    Task<CampaniaCrowdfundingId> AddAsync(CampaniaCrowdfunding entity, CancellationToken ct);
    Task UpdateAsync(CampaniaCrowdfunding entity, CancellationToken ct);
    Task<bool> ExistsByTituloAsync(string titulo, CancellationToken ct);
    Task<bool> ExistsByTituloExcludingIdAsync(string titulo, CampaniaCrowdfundingId excludeId, CancellationToken ct);
    Task<CampaniaCrowdfunding?> GetDetailByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct);
    Task<int> CountBackersByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct);

    // Dashboard methods
    Task<IReadOnlyList<CampaniaCrowdfunding>> GetMisCampaniasPaginatedAsync(
        ArtistaId artistaId,
        int? estadoCampaniaId,
        int page,
        int pageSize,
        CancellationToken ct);

    Task<int> CountMisCampaniasAsync(
        ArtistaId artistaId,
        int? estadoCampaniaId,
        CancellationToken ct);
}
