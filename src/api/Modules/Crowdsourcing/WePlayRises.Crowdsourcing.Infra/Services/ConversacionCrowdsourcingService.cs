using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Services;

public class ConversacionCrowdsourcingService : IConversacionCrowdsourcingService
{
    private readonly IConversacionCrowdsourcingRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<ConversacionCrowdsourcingService> _logger;

    public ConversacionCrowdsourcingService(
        IConversacionCrowdsourcingRepository repository,
        IRequestCacheService requestCache,
        ILogger<ConversacionCrowdsourcingService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ConversacionCrowdsourcing?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"conversacion:{id}",
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<bool> ExisteConversacionParaContextoAsync(
        string userIdCreador, string userIdDestinatario,
        Guid? necesidadId, Guid? acuerdoId, CancellationToken ct)
    {
        var cacheKey = $"conversacion-existe:{userIdCreador}:{userIdDestinatario}:{necesidadId}:{acuerdoId}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.ExisteConversacionParaContextoAsync(
                userIdCreador, userIdDestinatario, necesidadId, acuerdoId, ct));
    }

    public async Task<Guid> CreateAsync(ConversacionCrowdsourcing entity, CancellationToken ct)
    {
        return await _repository.AddAsync(entity, ct);
    }

    public async Task<(IReadOnlyList<ConversacionCrowdsourcing> Items, int TotalCount, int TotalNoLeidos)>
        GetConversacionesByUserIdAsync(
            string userId, string? filtroContexto, int page, int pageSize, CancellationToken ct)
    {
        return await _repository.GetConversacionesByUserIdAsync(
            userId, filtroContexto, page, pageSize, ct);
    }

    public async Task UpdateFechaUltimoMensajeAsync(
        Guid conversacionId, DateTime fechaUltimoMensaje, CancellationToken ct)
    {
        await _repository.UpdateFechaUltimoMensajeAsync(conversacionId, fechaUltimoMensaje, ct);
    }

    public async Task<int> GetTotalNoLeidosByUserIdAsync(string userId, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"conversacion-noleidos:{userId}",
            async () => await _repository.GetTotalNoLeidosByUserIdAsync(userId, ct));
    }
}
