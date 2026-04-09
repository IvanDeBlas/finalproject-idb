using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public interface IConversacionCrowdsourcingRepository
{
    Task<Guid> AddAsync(ConversacionCrowdsourcing entity, CancellationToken ct);
    Task<ConversacionCrowdsourcing?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<bool> ExisteConversacionParaContextoAsync(string userIdCreador, string userIdDestinatario, Guid? necesidadId, Guid? acuerdoId, CancellationToken ct);
    Task<(IReadOnlyList<ConversacionCrowdsourcing> Items, int TotalCount, int TotalNoLeidos)> GetConversacionesByUserIdAsync(string userId, string? filtroContexto, int page, int pageSize, CancellationToken ct);
    Task UpdateFechaUltimoMensajeAsync(Guid conversacionId, DateTime fechaUltimoMensaje, CancellationToken ct);
    Task<int> GetTotalNoLeidosByUserIdAsync(string userId, CancellationToken ct);
}
