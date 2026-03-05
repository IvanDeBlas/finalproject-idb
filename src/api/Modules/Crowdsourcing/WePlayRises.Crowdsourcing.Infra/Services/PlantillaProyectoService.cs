using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Services;

public class PlantillaProyectoService : IPlantillaProyectoService
{
    private readonly IPlantillaProyectoRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<PlantillaProyectoService> _logger;

    public PlantillaProyectoService(
        IPlantillaProyectoRepository repository,
        IRequestCacheService requestCache,
        ILogger<PlantillaProyectoService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PlantillaProyecto?> GetByIdAsync(PlantillaProyectoId id, CancellationToken ct)
    {
        var cacheKey = $"plantilla-proyecto:{id.Value}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<PlantillaProyecto?> GetByIdWithNecesidadesAsync(PlantillaProyectoId id, CancellationToken ct)
    {
        var cacheKey = $"plantilla-proyecto:necesidades:{id.Value}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdWithNecesidadesAsync(id, ct));
    }

    public async Task<IReadOnlyList<PlantillaProyecto>> GetAllActivosAsync(CancellationToken ct)
    {
        var cacheKey = "plantilla-proyecto:activos";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetAllActivosAsync(ct))
            ?? Array.Empty<PlantillaProyecto>();
    }

    public async Task<PlantillaProyectoId> CreateAsync(PlantillaProyecto entity, CancellationToken ct)
    {
        entity.FechaCreacion = DateTime.UtcNow;
        var id = await _repository.AddAsync(entity, ct);
        _logger.LogInformation("PlantillaProyecto {PlantillaId} created: {Nombre}", id.Value, entity.Nombre);
        return id;
    }

    public async Task UpdateAsync(PlantillaProyecto entity, CancellationToken ct)
    {
        await _repository.UpdateAsync(entity, ct);
        _logger.LogInformation("PlantillaProyecto {PlantillaId} updated", entity.Id.Value);
    }

    public async Task DeleteAsync(PlantillaProyectoId id, CancellationToken ct)
    {
        await _repository.DeleteAsync(id, ct);
        _logger.LogInformation("PlantillaProyecto {PlantillaId} soft deleted", id.Value);
    }
}
