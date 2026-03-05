using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Infra.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(
        IPedidoRepository repository,
        IRequestCacheService requestCache,
        ILogger<PedidoService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PedidoCrowdfunding?> GetByIdAsync(PedidoCrowdfundingId id, CancellationToken ct)
    {
        var cacheKey = $"pedido:{id.Value}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<IReadOnlyList<PedidoCrowdfunding>> GetAllAsync(CancellationToken ct)
    {
        return await _repository.GetAllAsync(ct);
    }

    public async Task<IReadOnlyList<PedidoCrowdfunding>> GetByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var cacheKey = $"pedidos:campania:{campaniaId.Value}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByCampaniaIdAsync(campaniaId, ct))
            ?? Array.Empty<PedidoCrowdfunding>();
    }

    public async Task<IReadOnlyList<PedidoCrowdfunding>> GetByUserIdAsync(string userId, CancellationToken ct)
    {
        var cacheKey = $"pedidos:user:{userId}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByUserIdAsync(userId, ct))
            ?? Array.Empty<PedidoCrowdfunding>();
    }

    public async Task<PedidoCrowdfundingId> CreateAsync(PedidoCrowdfunding pedido, CancellationToken ct)
    {
        pedido.FechaCreacion = DateTime.UtcNow;
        var id = await _repository.AddAsync(pedido, ct);

        _logger.LogInformation("Pedido {PedidoId} created for Campania {CampaniaId}",
            id.Value, pedido.CampaniaId.Value);

        return id;
    }

    public async Task UpdateAsync(PedidoCrowdfunding pedido, CancellationToken ct)
    {
        pedido.FechaActualizacion = DateTime.UtcNow;
        await _repository.UpdateAsync(pedido, ct);

        _logger.LogInformation("Pedido {PedidoId} updated", pedido.Id.Value);
    }

    public async Task<bool> ExistsAsync(PedidoCrowdfundingId id, CancellationToken ct)
    {
        var entity = await GetByIdAsync(id, ct);
        return entity != null;
    }

    public async Task<IReadOnlyList<PedidoCrowdfunding>> GetRecentByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, int limit, CancellationToken ct)
    {
        return await _repository.GetRecentByCampaniaIdAsync(campaniaId, limit, ct);
    }

    public async Task<int> CountByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var cacheKey = $"pedidos:campania:{campaniaId.Value}:count";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.CountByCampaniaIdAsync(campaniaId, ct));
    }

    public async Task<(decimal Total, decimal Average, decimal Min, decimal Max)> GetStatsByCampaniaIdAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        return await _repository.GetStatsByCampaniaIdAsync(campaniaId, ct);
    }
}
