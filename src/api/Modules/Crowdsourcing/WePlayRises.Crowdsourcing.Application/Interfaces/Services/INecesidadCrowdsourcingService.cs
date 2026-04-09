using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Services;

public interface INecesidadCrowdsourcingService
{
    Task<List<NecesidadCrowdsourcingId>> CreateManyAsync(List<NecesidadCrowdsourcing> entities, CancellationToken ct);
    Task<NecesidadCrowdsourcing?> GetByIdAsync(NecesidadCrowdsourcingId id, CancellationToken ct);
    Task<NecesidadCrowdsourcing?> GetByIdWithDetailsAsync(NecesidadCrowdsourcingId id, CancellationToken ct);
    Task<(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)> GetByArtistaIdPaginatedAsync(
        ArtistaId artistaId,
        int? estadoNecesidadId,
        string? search,
        int page,
        int pageSize,
        CancellationToken ct);
    Task<NecesidadCrowdsourcingId> CreateAsync(NecesidadCrowdsourcing entity, CancellationToken ct);
    Task<bool> UpdateAsync(NecesidadCrowdsourcing entity, CancellationToken ct);
    Task<int> CerrarAsync(NecesidadCrowdsourcingId necesidadId, string? motivo, CancellationToken ct);

    Task<(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)> GetPublicasPaginatedAsync(
        Interfaces.Repositories.NecesidadesPublicasFiltro filtro,
        ArtistaId? artistaDelUsuario,
        CancellationToken ct);

    Task<NecesidadCrowdsourcing?> GetPublicaByIdAsync(NecesidadCrowdsourcingId id, CancellationToken ct);
}
