using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Services;

public class NecesidadCrowdsourcingService : INecesidadCrowdsourcingService
{
    private readonly INecesidadCrowdsourcingRepository _repository;
    private readonly IPropuestaCrowdsourcingService _propuestaService;
    private readonly IRequestCacheService _requestCache;
    private readonly CrowdsourcingContext _context;
    private readonly ILogger<NecesidadCrowdsourcingService> _logger;

    public NecesidadCrowdsourcingService(
        INecesidadCrowdsourcingRepository repository,
        IPropuestaCrowdsourcingService propuestaService,
        IRequestCacheService requestCache,
        CrowdsourcingContext context,
        ILogger<NecesidadCrowdsourcingService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _propuestaService = propuestaService ?? throw new ArgumentNullException(nameof(propuestaService));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<NecesidadCrowdsourcingId>> CreateManyAsync(List<NecesidadCrowdsourcing> entities, CancellationToken ct)
    {
        var ids = await _repository.AddManyAsync(entities, ct);

        _logger.LogInformation(
            "Created {Count} NecesidadCrowdsourcing entities for ProyectoArtistico {ProyectoId}",
            ids.Count,
            entities.FirstOrDefault()?.ProyectoArtisticoId.Value);

        return ids;
    }

    public async Task<NecesidadCrowdsourcing?> GetByIdAsync(NecesidadCrowdsourcingId id, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"necesidad:{id.Value}",
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<NecesidadCrowdsourcing?> GetByIdWithDetailsAsync(NecesidadCrowdsourcingId id, CancellationToken ct)
    {
        return await _repository.GetByIdWithDetailsAsync(id, ct);
    }

    public async Task<(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)> GetByArtistaIdPaginatedAsync(
        ArtistaId artistaId,
        int? estadoNecesidadId,
        string? search,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        return await _repository.GetByArtistaIdPaginatedAsync(
            artistaId,
            estadoNecesidadId,
            search,
            page,
            pageSize,
            ct);
    }

    public async Task<NecesidadCrowdsourcingId> CreateAsync(NecesidadCrowdsourcing entity, CancellationToken ct)
    {
        var id = await _repository.AddAsync(entity, ct);

        _logger.LogInformation(
            "Created NecesidadCrowdsourcing {Id} for Artista {ArtistaId}",
            id.Value,
            entity.ArtistaId.Value);

        return id;
    }

    public async Task<bool> UpdateAsync(NecesidadCrowdsourcing entity, CancellationToken ct)
    {
        await _repository.UpdateAsync(entity, ct);

        _logger.LogInformation(
            "Updated NecesidadCrowdsourcing {Id}",
            entity.Id.Value);

        return true;
    }

    public async Task<(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)> GetPublicasPaginatedAsync(
        Application.Interfaces.Repositories.NecesidadesPublicasFiltro filtro,
        ArtistaId? artistaDelUsuario,
        CancellationToken ct)
    {
        return await _repository.GetPublicasPaginatedAsync(filtro, artistaDelUsuario, ct);
    }

    public async Task<NecesidadCrowdsourcing?> GetPublicaByIdAsync(NecesidadCrowdsourcingId id, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"necesidad:publica:{id.Value}",
            async () => await _repository.GetPublicaByIdAsync(id, ct));
    }

    public async Task<int> CerrarAsync(NecesidadCrowdsourcingId necesidadId, string? motivo, CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            var necesidad = await _repository.GetByIdAsync(necesidadId, ct);
            if (necesidad == null)
            {
                _logger.LogWarning("Attempted to close non-existent necesidad {Id}", necesidadId.Value);
                return 0;
            }

            necesidad.EstadoNecesidadId = 3; // 3 = Cerrada
            necesidad.MotivoCierre = motivo;
            necesidad.FechaActualizacion = DateTime.UtcNow;

            await _repository.UpdateAsync(necesidad, ct);

            var propuestasRechazadas = await _propuestaService.RechazarPropuestasPendientesAsync(necesidadId, ct);

            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "Closed NecesidadCrowdsourcing {Id} with {Count} proposals rejected",
                necesidadId.Value,
                propuestasRechazadas);

            return propuestasRechazadas;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
