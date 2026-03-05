using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Services;

public interface IPropuestaCrowdsourcingService
{
    Task<int> RechazarPropuestasPendientesAsync(NecesidadCrowdsourcingId necesidadId, CancellationToken ct);

    Task<PropuestaCrowdsourcing?> GetByIdAsync(PropuestaCrowdsourcingId id, CancellationToken ct);

    Task<bool> ExistePropuestaActivaAsync(NecesidadCrowdsourcingId necesidadId, string userId, CancellationToken ct);

    Task<PropuestaCrowdsourcingId> CreateAsync(PropuestaCrowdsourcing entity, CancellationToken ct);

    Task<bool> RetirarAsync(PropuestaCrowdsourcingId id, CancellationToken ct);

    Task<bool> RechazarAsync(PropuestaCrowdsourcingId id, string? motivo, CancellationToken ct);

    Task<(IReadOnlyList<PropuestaCrowdsourcing> Items, int TotalCount)> GetByUserIdPaginatedAsync(
        string userId,
        int? estadoPropuestaId,
        int page,
        int pageSize,
        CancellationToken ct);
}
