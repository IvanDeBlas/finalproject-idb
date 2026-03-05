using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public interface IMensajeCrowdsourcingRepository
{
    Task<Guid> AddAsync(MensajeCrowdsourcing entity, CancellationToken ct);
    Task<(IReadOnlyList<MensajeCrowdsourcing> Items, int TotalCount)> GetByConversacionIdPaginatedAsync(Guid conversacionId, int page, int pageSize, CancellationToken ct);
    Task<int> MarcarLeidosByConversacionAsync(Guid conversacionId, string userIdDestinatario, DateTime fechaLeido, CancellationToken ct);
}
