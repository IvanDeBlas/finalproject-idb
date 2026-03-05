using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Services;

public interface IMensajeCrowdsourcingService
{
    Task<MensajeCrowdsourcing> SendAsync(MensajeCrowdsourcing entity, Guid conversacionId, CancellationToken ct);
    Task<(IReadOnlyList<MensajeCrowdsourcing> Items, int TotalCount)> GetByConversacionIdPaginatedAsync(Guid conversacionId, int page, int pageSize, CancellationToken ct);
    Task<int> MarcarLeidosAsync(Guid conversacionId, string userIdQueAbre, CancellationToken ct);
    Task<int> GetTotalNoLeidosAsync(string userId, CancellationToken ct);
}
