using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public interface INecesidadCrowdsourcingRepository
{
    Task<NecesidadCrowdsourcingId> AddAsync(NecesidadCrowdsourcing entity, CancellationToken ct);
    Task<List<NecesidadCrowdsourcingId>> AddManyAsync(List<NecesidadCrowdsourcing> entities, CancellationToken ct);
    Task<NecesidadCrowdsourcing?> GetByIdAsync(NecesidadCrowdsourcingId id, CancellationToken ct);
    Task<NecesidadCrowdsourcing?> GetByIdWithDetailsAsync(NecesidadCrowdsourcingId id, CancellationToken ct);
    Task<(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)> GetByArtistaIdPaginatedAsync(
        ArtistaId artistaId,
        int? estadoNecesidadId,
        string? search,
        int page,
        int pageSize,
        CancellationToken ct);
    Task UpdateAsync(NecesidadCrowdsourcing entity, CancellationToken ct);

    Task<(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)> GetPublicasPaginatedAsync(
        NecesidadesPublicasFiltro filtro,
        ArtistaId? artistaDelUsuario,
        CancellationToken ct);

    Task<NecesidadCrowdsourcing?> GetPublicaByIdAsync(NecesidadCrowdsourcingId id, CancellationToken ct);
}
