using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Services;

public class MensajeCrowdsourcingService : IMensajeCrowdsourcingService
{
    private readonly IMensajeCrowdsourcingRepository _repository;
    private readonly IConversacionCrowdsourcingRepository _conversacionRepository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<MensajeCrowdsourcingService> _logger;

    public MensajeCrowdsourcingService(
        IMensajeCrowdsourcingRepository repository,
        IConversacionCrowdsourcingRepository conversacionRepository,
        IRequestCacheService requestCache,
        ILogger<MensajeCrowdsourcingService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _conversacionRepository = conversacionRepository ?? throw new ArgumentNullException(nameof(conversacionRepository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<MensajeCrowdsourcing> SendAsync(
        MensajeCrowdsourcing entity, Guid conversacionId, CancellationToken ct)
    {
        var id = await _repository.AddAsync(entity, ct);
        entity.Id = id;

        await _conversacionRepository.UpdateFechaUltimoMensajeAsync(
            conversacionId, entity.FechaCreacion, ct);

        return entity;
    }

    public async Task<(IReadOnlyList<MensajeCrowdsourcing> Items, int TotalCount)>
        GetByConversacionIdPaginatedAsync(
            Guid conversacionId, int page, int pageSize, CancellationToken ct)
    {
        return await _repository.GetByConversacionIdPaginatedAsync(
            conversacionId, page, pageSize, ct);
    }

    public async Task<int> MarcarLeidosAsync(
        Guid conversacionId, string userIdQueAbre, CancellationToken ct)
    {
        var fechaLeido = DateTime.UtcNow;
        return await _repository.MarcarLeidosByConversacionAsync(
            conversacionId, userIdQueAbre, fechaLeido, ct);
    }

    public async Task<int> GetTotalNoLeidosAsync(string userId, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"mensajes-noleidos:{userId}",
            async () => await _conversacionRepository.GetTotalNoLeidosByUserIdAsync(userId, ct));
    }
}
