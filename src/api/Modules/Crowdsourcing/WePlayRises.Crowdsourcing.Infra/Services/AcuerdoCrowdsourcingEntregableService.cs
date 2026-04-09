using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Services;

public class AcuerdoCrowdsourcingEntregableService : IAcuerdoCrowdsourcingEntregableService
{
    private readonly IAcuerdoCrowdsourcingEntregableRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<AcuerdoCrowdsourcingEntregableService> _logger;

    public AcuerdoCrowdsourcingEntregableService(
        IAcuerdoCrowdsourcingEntregableRepository repository,
        IRequestCacheService requestCache,
        ILogger<AcuerdoCrowdsourcingEntregableService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AcuerdoCrowdsourcingEntregable?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"entregable:{id}",
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<AcuerdoCrowdsourcingEntregable?> GetByIdWithAcuerdoAsync(Guid id, CancellationToken ct)
    {
        return await _repository.GetByIdWithAcuerdoAsync(id, ct);
    }

    public async Task<Guid> CreateAsync(AcuerdoCrowdsourcingEntregable entity, CancellationToken ct)
    {
        entity.FechaCreacion = DateTime.UtcNow;
        entity.EstadoEntregableId = EstadoEntregableConstants.Entregado;

        var id = await _repository.AddAsync(entity, ct);

        _logger.LogInformation(
            "Created entregable {EntregableId} for acuerdo {AcuerdoId}",
            id,
            entity.AcuerdoId.Value);

        return id;
    }

    public async Task<bool> AprobarAsync(Guid id, string? comentario, CancellationToken ct)
    {
        var entregable = await _repository.GetByIdAsync(id, ct);
        if (entregable == null)
        {
            return false;
        }

        entregable.EstadoEntregableId = EstadoEntregableConstants.Aprobado;
        entregable.FechaAprobacion = DateTime.UtcNow;
        entregable.ComentarioAprobacion = comentario;
        entregable.FechaActualizacion = DateTime.UtcNow;

        await _repository.UpdateAsync(entregable, ct);

        _logger.LogInformation(
            "Approved entregable {EntregableId}",
            id);

        return true;
    }

    public async Task<bool> RechazarAsync(Guid id, string comentario, CancellationToken ct)
    {
        var entregable = await _repository.GetByIdAsync(id, ct);
        if (entregable == null)
        {
            return false;
        }

        entregable.EstadoEntregableId = EstadoEntregableConstants.Rechazado;
        entregable.ComentarioRechazo = comentario;
        entregable.FechaActualizacion = DateTime.UtcNow;

        await _repository.UpdateAsync(entregable, ct);

        _logger.LogInformation(
            "Rejected entregable {EntregableId}",
            id);

        return true;
    }

    public async Task<bool> TodosAprobadosEnMilestoneAsync(Guid milestoneId, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"entregable:todos-aprobados:{milestoneId}",
            async () => await _repository.TodosAprobadosEnMilestoneAsync(milestoneId, ct));
    }
}
