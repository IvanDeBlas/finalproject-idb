using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public interface IPropuestaCrowdsourcingRepository
{
    Task<IReadOnlyList<PropuestaCrowdsourcing>> GetPendientesByNecesidadIdAsync(
        NecesidadCrowdsourcingId necesidadId,
        CancellationToken ct);

    Task UpdateManyAsync(IEnumerable<PropuestaCrowdsourcing> propuestas, CancellationToken ct);

    Task<PropuestaCrowdsourcing?> GetByIdAsync(PropuestaCrowdsourcingId id, CancellationToken ct);

    Task<PropuestaCrowdsourcingId> AddAsync(PropuestaCrowdsourcing entity, CancellationToken ct);

    Task UpdateAsync(PropuestaCrowdsourcing entity, CancellationToken ct);

    Task<bool> ExisteAsync(NecesidadCrowdsourcingId necesidadId, string userId, CancellationToken ct);

    Task<(IReadOnlyList<PropuestaCrowdsourcing> Items, int TotalCount)> GetByUserIdPaginatedAsync(
        string userId,
        int? estadoPropuestaId,
        int page,
        int pageSize,
        CancellationToken ct);
}
