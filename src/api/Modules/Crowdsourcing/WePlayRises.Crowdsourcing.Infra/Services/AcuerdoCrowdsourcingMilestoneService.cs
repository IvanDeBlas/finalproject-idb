using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Services;

public class AcuerdoCrowdsourcingMilestoneService : IAcuerdoCrowdsourcingMilestoneService
{
    private readonly IAcuerdoCrowdsourcingMilestoneRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<AcuerdoCrowdsourcingMilestoneService> _logger;

    public AcuerdoCrowdsourcingMilestoneService(
        IAcuerdoCrowdsourcingMilestoneRepository repository,
        IRequestCacheService requestCache,
        ILogger<AcuerdoCrowdsourcingMilestoneService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AcuerdoCrowdsourcingMilestone?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"milestone:{id}",
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<decimal> GetSumaImportesAsync(AcuerdoCrowdsourcingId acuerdoId, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"milestone:suma:{acuerdoId.Value}",
            async () => await _repository.GetSumaImportesByAcuerdoIdAsync(acuerdoId, ct));
    }

    public async Task<decimal> GetSumaImportesExcluyendoAsync(AcuerdoCrowdsourcingId acuerdoId, Guid excludeId, CancellationToken ct)
    {
        return await _repository.GetSumaImportesExcluyendoAsync(acuerdoId, excludeId, ct);
    }

    public async Task<Guid> CreateAsync(AcuerdoCrowdsourcingMilestone entity, CancellationToken ct)
    {
        entity.FechaCreacion = DateTime.UtcNow;

        var maxOrden = await _repository.GetMaxOrdenAsync(entity.AcuerdoId, ct);
        entity.Orden = maxOrden + 1;

        var id = await _repository.AddAsync(entity, ct);

        _logger.LogInformation(
            "Created milestone {MilestoneId} for acuerdo {AcuerdoId} with orden {Orden}",
            id,
            entity.AcuerdoId.Value,
            entity.Orden);

        return id;
    }

    public async Task<bool> UpdateAsync(AcuerdoCrowdsourcingMilestone entity, CancellationToken ct)
    {
        await _repository.UpdateAsync(entity, ct);

        _logger.LogInformation(
            "Updated milestone {MilestoneId}",
            entity.Id);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        await _repository.DeleteAsync(id, ct);

        _logger.LogInformation(
            "Deleted milestone {MilestoneId}",
            id);

        return true;
    }

    public async Task<bool> TieneEntregablesAsync(Guid milestoneId, CancellationToken ct)
    {
        return await _repository.TieneEntregablesAsync(milestoneId, ct);
    }
}
