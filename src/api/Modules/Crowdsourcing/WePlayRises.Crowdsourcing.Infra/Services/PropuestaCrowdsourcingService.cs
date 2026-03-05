using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Services;

public class PropuestaCrowdsourcingService : IPropuestaCrowdsourcingService
{
    private readonly IPropuestaCrowdsourcingRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<PropuestaCrowdsourcingService> _logger;

    public PropuestaCrowdsourcingService(
        IPropuestaCrowdsourcingRepository repository,
        IRequestCacheService requestCache,
        ILogger<PropuestaCrowdsourcingService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> RechazarPropuestasPendientesAsync(
        NecesidadCrowdsourcingId necesidadId,
        CancellationToken ct)
    {
        var propuestas = await _repository.GetPendientesByNecesidadIdAsync(necesidadId, ct);

        if (propuestas.Count == 0)
        {
            return 0;
        }

        foreach (var propuesta in propuestas)
        {
            propuesta.EstadoPropuestaId = EstadoPropuestaConstants.Rechazada;
            propuesta.FechaActualizacion = DateTime.UtcNow;
        }

        await _repository.UpdateManyAsync(propuestas, ct);

        _logger.LogInformation(
            "Rejected {Count} pending proposals for necesidad {NecesidadId}",
            propuestas.Count,
            necesidadId.Value);

        return propuestas.Count;
    }

    public async Task<PropuestaCrowdsourcing?> GetByIdAsync(PropuestaCrowdsourcingId id, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"propuesta:{id.Value}",
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<bool> ExistePropuestaActivaAsync(
        NecesidadCrowdsourcingId necesidadId,
        string userId,
        CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"propuesta:existe:{necesidadId.Value}:{userId}",
            async () => await _repository.ExisteAsync(necesidadId, userId, ct));
    }

    public async Task<PropuestaCrowdsourcingId> CreateAsync(PropuestaCrowdsourcing entity, CancellationToken ct)
    {
        var id = await _repository.AddAsync(entity, ct);

        _logger.LogInformation(
            "Created PropuestaCrowdsourcing {Id} for necesidad {NecesidadId} by user {UserId}",
            id.Value,
            entity.NecesidadId.Value,
            entity.UserId);

        return id;
    }

    public async Task<bool> RetirarAsync(PropuestaCrowdsourcingId id, CancellationToken ct)
    {
        var propuesta = await _repository.GetByIdAsync(id, ct);
        if (propuesta == null)
        {
            return false;
        }

        propuesta.EstadoPropuestaId = EstadoPropuestaConstants.Retirada;
        propuesta.FechaActualizacion = DateTime.UtcNow;

        await _repository.UpdateAsync(propuesta, ct);

        _logger.LogInformation(
            "Propuesta {PropuestaId} retracted by user {UserId}",
            id.Value,
            propuesta.UserId);

        return true;
    }

    public async Task<bool> RechazarAsync(PropuestaCrowdsourcingId id, string? motivo, CancellationToken ct)
    {
        var propuesta = await _repository.GetByIdAsync(id, ct);
        if (propuesta == null)
        {
            return false;
        }

        propuesta.EstadoPropuestaId = EstadoPropuestaConstants.Rechazada;
        propuesta.MotivoRechazo = motivo;
        propuesta.FechaActualizacion = DateTime.UtcNow;

        await _repository.UpdateAsync(propuesta, ct);

        _logger.LogInformation(
            "Propuesta {PropuestaId} rejected",
            id.Value);

        return true;
    }

    public async Task<(IReadOnlyList<PropuestaCrowdsourcing> Items, int TotalCount)> GetByUserIdPaginatedAsync(
        string userId,
        int? estadoPropuestaId,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        return await _repository.GetByUserIdPaginatedAsync(userId, estadoPropuestaId, page, pageSize, ct);
    }
}
