using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Interfaces;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.Crowdpromotion.Infra.Context;

namespace WePlayRises.Crowdpromotion.Infra.Services;

public class PromoProgramaService : IPromoProgramaService
{
    private readonly IPromoProgramaRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<PromoProgramaService> _logger;
    private readonly CrowdpromotionContext _context;

    private static readonly HashSet<int> ValidTipoPromoIds = new() { 1, 2, 3, 4 };
    private static readonly HashSet<int> ValidTipoEventoPromoIds = new() { 1, 2, 3, 4, 5, 6 };
    private static readonly HashSet<int> ValidTipoRewardIds = new() { 1, 2, 3 };
    private static readonly HashSet<int> ValidMonedaIds = new() { 1, 2 };

    public PromoProgramaService(
        IPromoProgramaRepository repository,
        IRequestCacheService requestCache,
        ILogger<PromoProgramaService> logger,
        CrowdpromotionContext context)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
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

    public async Task<PromoPrograma?> GetByIdAsync(PromoProgramaId id, CancellationToken ct)
    {
        var cacheKey = $"promo-programa:{id.Value}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<PromoPrograma?> GetByIdWithTareasAsync(PromoProgramaId id, CancellationToken ct)
    {
        return await _repository.GetByIdWithTareasAsync(id, ct);
    }

    public async Task<PromoPrograma?> GetByIdWithFullDetailAsync(PromoProgramaId id, CancellationToken ct)
    {
        return await _repository.GetByIdWithFullDetailAsync(id, ct);
    }

    public async Task<(IReadOnlyList<PromoPrograma> Items, int TotalCount)> GetMisProgramasAsync(
        ArtistaId artistaId, bool? esActivo, int page, int pageSize, CancellationToken ct)
    {
        return await _repository.GetByArtistaIdPagedAsync(artistaId, esActivo, page, pageSize, ct);
    }

    public async Task<bool> ExisteCodigoTrackingAsync(
        string codigoTracking, ArtistaId artistaId, CancellationToken ct)
    {
        var cacheKey = $"promo-programa:codigo:{codigoTracking}:{artistaId.Value}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.ExistsByCodigoTrackingAndArtistaAsync(codigoTracking, artistaId, ct));
    }

    public async Task<bool> ExisteCodigoTrackingParaEdicionAsync(
        string codigoTracking, ArtistaId artistaId, PromoProgramaId excludeId, CancellationToken ct)
    {
        return await _repository.ExistsByCodigoTrackingAndArtistaExcludingIdAsync(
            codigoTracking, artistaId, excludeId, ct);
    }

    public async Task<bool> CodigoTrackingExistsAsync(string? userId, string? codigo, Guid? excludeId, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(codigo))
            return false;

        var artistaId = await GetArtistaIdByUserIdAsync(userId, ct);
        if (artistaId == null)
            return false;

        if (excludeId.HasValue)
            return await ExisteCodigoTrackingParaEdicionAsync(codigo, artistaId.Value, new PromoProgramaId(excludeId.Value), ct);

        return await ExisteCodigoTrackingAsync(codigo, artistaId.Value, ct);
    }

    public async Task<PromoProgramaId> CreateWithTareasAsync(
        PromoPrograma programa, IReadOnlyList<PromoTarea> tareas, CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            var programaId = await _repository.AddAsync(programa, ct);

            if (tareas.Count > 0)
            {
                foreach (var tarea in tareas)
                {
                    tarea.ProgramaId = programaId;
                }
                await _context.Tareas.AddRangeAsync(tareas, ct);
                await _context.SaveChangesAsync(ct);
            }

            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "PromoPrograma {ProgramaId} created with {TareasCount} tareas",
                programaId.Value, tareas.Count);

            return programaId;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task UpdateWithTareasAsync(
        PromoPrograma programa,
        IReadOnlyList<PromoTarea> tareasNuevas,
        IReadOnlyList<PromoTarea> tareasActualizar,
        IReadOnlyList<Guid> tareasDesactivar,
        CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            programa.FechaActualizacion = DateTime.UtcNow;
            _context.Programas.Update(programa);

            if (tareasDesactivar.Count > 0)
            {
                var tareasToDeactivate = await _context.Tareas
                    .Where(t => tareasDesactivar.Contains(t.Id))
                    .ToListAsync(ct);

                foreach (var tarea in tareasToDeactivate)
                {
                    tarea.EsActivo = false;
                }
            }

            if (tareasActualizar.Count > 0)
            {
                foreach (var tarea in tareasActualizar)
                {
                    _context.Entry(tarea).State = EntityState.Modified;
                }
            }

            if (tareasNuevas.Count > 0)
            {
                foreach (var tarea in tareasNuevas)
                {
                    tarea.ProgramaId = programa.Id;
                }
                await _context.Tareas.AddRangeAsync(tareasNuevas, ct);
            }

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "PromoPrograma {ProgramaId} updated. New tareas: {New}, Updated: {Updated}, Deactivated: {Deactivated}",
                programa.Id.Value, tareasNuevas.Count, tareasActualizar.Count, tareasDesactivar.Count);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<int> DesactivarWithTareasAsync(PromoProgramaId id, CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            var programa = await _repository.GetByIdWithTareasAsync(id, ct);
            if (programa == null)
                throw new InvalidOperationException($"PromoPrograma {id.Value} not found");

            programa.EsActivo = false;
            programa.FechaActualizacion = DateTime.UtcNow;

            var tareasDesactivadas = 0;
            foreach (var tarea in programa.Tareas.Where(t => t.EsActivo))
            {
                tarea.EsActivo = false;
                tareasDesactivadas++;
            }

            await _repository.UpdateAsync(programa, ct);

            if (tareasDesactivadas > 0)
            {
                await _context.SaveChangesAsync(ct);
            }

            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "PromoPrograma {ProgramaId} deactivated. Tareas deactivated: {Count}",
                id.Value, tareasDesactivadas);

            return tareasDesactivadas;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public Task<bool> TipoPromoExistsAsync(int tipoPromoId, CancellationToken ct)
    {
        return Task.FromResult(ValidTipoPromoIds.Contains(tipoPromoId));
    }

    public Task<bool> TipoEventoPromoExistsAsync(int tipoEventoPromoId, CancellationToken ct)
    {
        return Task.FromResult(ValidTipoEventoPromoIds.Contains(tipoEventoPromoId));
    }

    public Task<bool> TipoRewardExistsAsync(int tipoRewardId, CancellationToken ct)
    {
        return Task.FromResult(ValidTipoRewardIds.Contains(tipoRewardId));
    }

    public Task<bool> MonedaExistsAsync(int monedaId, CancellationToken ct)
    {
        return Task.FromResult(ValidMonedaIds.Contains(monedaId));
    }

    public async Task<bool> VerificarCampaniaPertenece(
        ArtistaId artistaId, Guid campaniaCrowdfundingId, CancellationToken ct)
    {
        var exists = await _context.Database
            .SqlQueryRaw<int>(
                "SELECT COUNT(1) AS [Value] FROM CampaniaCrowdfunding WHERE Id = {0} AND Artista_Id = {1}",
                campaniaCrowdfundingId, artistaId.Value)
            .FirstOrDefaultAsync(ct);

        return exists > 0;
    }

    public async Task<bool> VerificarProyectoPertenece(
        ArtistaId artistaId, Guid proyectoArtisticoId, CancellationToken ct)
    {
        var exists = await _context.Database
            .SqlQueryRaw<int>(
                "SELECT COUNT(1) AS [Value] FROM ProyectoArtistico WHERE Id = {0} AND Artista_Id = {1}",
                proyectoArtisticoId, artistaId.Value)
            .FirstOrDefaultAsync(ct);

        return exists > 0;
    }

    public async Task<bool> TareaConCompletadosAsync(Guid tareaId, CancellationToken ct)
    {
        return await _context.TareaPromotores
            .AnyAsync(tp => tp.TareaId == tareaId, ct);
    }

    public async Task<PromoProgramaResumenDto> GetResumenAsync(PromoProgramaId programaId, CancellationToken ct)
    {
        var promotores = await _context.ProgramaPromotores
            .Where(pp => pp.ProgramaId == programaId)
            .AsNoTracking()
            .ToListAsync(ct);

        var eventos = await _context.Eventos
            .Where(e => e.ProgramaId == programaId)
            .AsNoTracking()
            .ToListAsync(ct);

        return new PromoProgramaResumenDto
        {
            TotalPromotoresAprobados = promotores.Count(p => p.EsActivo),
            TotalPromotoresPendientes = promotores.Count(p => !p.EsActivo && !p.FechaBaja.HasValue),
            TotalEventos = eventos.Count,
            TotalConversiones = eventos.Count(e => e.TipoEventoId == 6),
            ValorTotalGenerado = eventos.Where(e => e.ImporteAsociado.HasValue).Sum(e => e.ImporteAsociado!.Value)
        };
    }
}
