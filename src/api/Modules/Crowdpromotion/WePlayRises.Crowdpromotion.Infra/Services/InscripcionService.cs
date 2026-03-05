using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Interfaces;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.Crowdpromotion.Infra.Context;

namespace WePlayRises.Crowdpromotion.Infra.Services;

public class InscripcionService : IInscripcionService
{
    private readonly IPromoProgramaPromotorRepository _repository;
    private readonly IPromoProgramaRepository _programaRepository;
    private readonly IPromotorRepository _promotorRepository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<InscripcionService> _logger;
    private readonly CrowdpromotionContext _context;

    public InscripcionService(
        IPromoProgramaPromotorRepository repository,
        IPromoProgramaRepository programaRepository,
        IPromotorRepository promotorRepository,
        IRequestCacheService requestCache,
        ILogger<InscripcionService> logger,
        CrowdpromotionContext context)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _programaRepository = programaRepository ?? throw new ArgumentNullException(nameof(programaRepository));
        _promotorRepository = promotorRepository ?? throw new ArgumentNullException(nameof(promotorRepository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // Identity resolution
    public async Task<Promotor?> GetPromotorByUserIdAsync(string userId, CancellationToken ct)
    {
        var cacheKey = $"promotor:userid:{userId}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _promotorRepository.GetByUserIdAsync(userId, ct));
    }

    public async Task<ArtistaId?> GetArtistaIdByUserIdAsync(string userId, CancellationToken ct)
    {
        var cacheKey = $"artista:userid:{userId}";
        return await _requestCache.GetOrAddAsync<ArtistaId?>(
            cacheKey,
            async () =>
            {
                var artistaId = await _context.Database
                    .SqlQueryRaw<Guid>("SELECT Id AS [Value] FROM Artista WHERE UserId = {0}", userId)
                    .FirstOrDefaultAsync(ct);

                return artistaId != default ? new ArtistaId(artistaId) : null;
            });
    }

    // PromoPrograma access
    public async Task<PromoPrograma?> GetProgramaByIdAsync(PromoProgramaId id, CancellationToken ct)
    {
        var cacheKey = $"promo-programa:{id.Value}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _programaRepository.GetByIdAsync(id, ct));
    }

    public async Task<(IReadOnlyList<PromoPrograma> Items, int TotalCount)> GetProgramasActivosAsync(
        string? artistaNombre, int? tipoPromoId, int page, int pageSize, PromotorId promotorId, CancellationToken ct)
    {
        return await _programaRepository.GetActivosPagedAsync(artistaNombre, tipoPromoId, page, pageSize, promotorId, ct);
    }

    // Inscripcion CRUD
    public async Task<PromoProgramaPromotor?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var cacheKey = $"ppp:id:{id}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<PromoProgramaPromotor?> GetByIdWithPromotorAsync(Guid id, CancellationToken ct)
    {
        return await _repository.GetByIdWithPromotorAsync(id, ct);
    }

    public async Task<PromoProgramaPromotor?> GetByPromotorYProgramaAsync(
        PromotorId promotorId, PromoProgramaId programaId, CancellationToken ct)
    {
        var cacheKey = $"ppp:promotor:{promotorId.Value}:programa:{programaId.Value}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByPromotorAndProgramaAsync(promotorId, programaId, ct));
    }

    public async Task<Guid> CreateAsync(PromoProgramaPromotor inscripcion, CancellationToken ct)
    {
        var id = await _repository.AddAsync(inscripcion, ct);
        _logger.LogInformation("Inscripcion {Id} created for PromotorId {PromotorId} in ProgramaId {ProgramaId}",
            id, inscripcion.PromotorId.Value, inscripcion.ProgramaId.Value);
        return id;
    }

    public async Task AprobarAsync(PromoProgramaPromotor inscripcion, CancellationToken ct)
    {
        await _repository.UpdateAsync(inscripcion, ct);
        _logger.LogInformation("Inscripcion {Id} aprobada. CodigoReferido: {Codigo}",
            inscripcion.Id, inscripcion.CodigoReferido);
    }

    public async Task RechazarAsync(Guid inscripcionId, CancellationToken ct)
    {
        await _repository.DeleteAsync(inscripcionId, ct);
        _logger.LogInformation("Inscripcion {Id} rechazada y eliminada", inscripcionId);
    }

    public async Task BloquearAsync(PromoProgramaPromotor inscripcion, CancellationToken ct)
    {
        await _repository.UpdateAsync(inscripcion, ct);
        _logger.LogInformation("Inscripcion {Id} bloqueada", inscripcion.Id);
    }

    public async Task DarDeBajaAsync(PromoProgramaPromotor inscripcion, CancellationToken ct)
    {
        await _repository.UpdateAsync(inscripcion, ct);
        _logger.LogInformation("Inscripcion {Id} dada de baja", inscripcion.Id);
    }

    // Queries
    public async Task<(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)> GetInscripcionesPorProgramaAsync(
        PromoProgramaId programaId, string? estado, int page, int pageSize, CancellationToken ct)
    {
        return await _repository.GetPagedByProgramaAsync(programaId, estado, page, pageSize, ct);
    }

    public async Task<(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)> GetMisInscripcionesAsync(
        PromotorId promotorId, int page, int pageSize, CancellationToken ct)
    {
        return await _repository.GetPagedByPromotorAsync(promotorId, page, pageSize, ct);
    }

    // Uniqueness check
    public async Task<bool> CodigoReferidoExistsAsync(string codigoReferido, CancellationToken ct)
    {
        return await _repository.ExistsByCodigoReferidoAsync(codigoReferido, ct);
    }
}
