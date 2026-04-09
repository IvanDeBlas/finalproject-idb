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

public class PromoTareaService : IPromoTareaService
{
    private readonly IPromoTareaRepository _tareaRepository;
    private readonly IPromoTareaPromotorRepository _tareaPromotorRepository;
    private readonly IPromotorRepository _promotorRepository;
    private readonly IPromoProgramaPromotorRepository _programaPromotorRepository;
    private readonly IPromoProgramaRepository _programaRepository;
    private readonly IPromotorWalletRepository _walletRepository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<PromoTareaService> _logger;
    private readonly CrowdpromotionContext _context;

    private static readonly Dictionary<int, string> TipoEventoPromoNombres = new()
    {
        { 1, "Click" }, { 2, "PageView" }, { 3, "Share" }, { 4, "Post" }, { 5, "Signup" }, { 6, "Backing" }
    };

    private static readonly Dictionary<int, string> TipoRewardNombres = new()
    {
        { 1, "Dinero" }, { 2, "Puntos" }, { 3, "Mixto" }
    };

    private static readonly Dictionary<int, string> MonedaNombres = new()
    {
        { 1, "EUR" }, { 2, "USD" }
    };

    private static readonly Dictionary<int, string> EstadoTareaNombres = new()
    {
        { 1, "Pendiente" }, { 2, "Completada" }, { 3, "Validada" }, { 4, "Rechazada" }
    };

    private static readonly Dictionary<int, string> TipoPromotorNombres = new()
    {
        { 1, "Fan Embajador" }, { 2, "Influencer" }, { 3, "Medio / Blog" }, { 4, "Profesional Marketing" }
    };

    public PromoTareaService(
        IPromoTareaRepository tareaRepository,
        IPromoTareaPromotorRepository tareaPromotorRepository,
        IPromotorRepository promotorRepository,
        IPromoProgramaPromotorRepository programaPromotorRepository,
        IPromoProgramaRepository programaRepository,
        IPromotorWalletRepository walletRepository,
        IRequestCacheService requestCache,
        ILogger<PromoTareaService> logger,
        CrowdpromotionContext context)
    {
        _tareaRepository = tareaRepository ?? throw new ArgumentNullException(nameof(tareaRepository));
        _tareaPromotorRepository = tareaPromotorRepository ?? throw new ArgumentNullException(nameof(tareaPromotorRepository));
        _promotorRepository = promotorRepository ?? throw new ArgumentNullException(nameof(promotorRepository));
        _programaPromotorRepository = programaPromotorRepository ?? throw new ArgumentNullException(nameof(programaPromotorRepository));
        _programaRepository = programaRepository ?? throw new ArgumentNullException(nameof(programaRepository));
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
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
    public async Task<PromoPrograma?> GetProgramaByIdAsync(PromoProgramaId programaId, CancellationToken ct)
    {
        var cacheKey = $"promo-programa:{programaId.Value}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _programaRepository.GetByIdAsync(programaId, ct));
    }

    // Inscripcion access
    public async Task<PromoProgramaPromotor?> GetInscripcionAprobadaAsync(
        PromotorId promotorId, PromoProgramaId programaId, CancellationToken ct)
    {
        var cacheKey = $"inscripcion:{promotorId.Value}:{programaId.Value}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _programaPromotorRepository.GetByPromotorAndProgramaAsync(promotorId, programaId, ct));
    }

    // PromoTarea access
    public async Task<PromoTarea?> GetTareaByIdAndProgramaAsync(
        Guid tareaId, PromoProgramaId programaId, CancellationToken ct)
    {
        var cacheKey = $"promo-tarea:{tareaId}:{programaId.Value}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _tareaRepository.GetByIdAndProgramaAsync(tareaId, programaId, ct));
    }

    // PromoTareaPromotor access
    public async Task<PromoTareaPromotor?> GetTareaPromotorByIdAsync(Guid tareaPromotorId, CancellationToken ct)
    {
        return await _tareaPromotorRepository.GetByIdWithTareaAndProgramaAsync(tareaPromotorId, ct);
    }

    public async Task<int> ContarCompletadosActivosAsync(Guid tareaId, Guid programaPromotorId, CancellationToken ct)
    {
        return await _tareaPromotorRepository.CountCompletadosActivosAsync(tareaId, programaPromotorId, ct);
    }

    public async Task<PromoTareaPromotor?> GetUltimoCompletadoAsync(
        Guid tareaId, Guid programaPromotorId, CancellationToken ct)
    {
        return await _tareaPromotorRepository.GetUltimoByTareaAndProgramaPromotorAsync(
            tareaId, programaPromotorId, ct);
    }

    // Orchestration: GET mis-tareas
    public async Task<MisTareasResponseDto> GetTareasActivasConEstadoAsync(
        PromoProgramaId programaId, Guid programaPromotorId, CancellationToken ct)
    {
        var programa = await GetProgramaByIdAsync(programaId, ct);
        var tareas = await _tareaRepository.GetTareasByProgramaAsync(programaId, ct);

        // Load all completados for this promotor in one query
        var completados = await _context.TareaPromotores
            .Where(x => x.ProgramaPromotorId == programaPromotorId)
            .AsNoTracking()
            .ToListAsync(ct);

        var completadosPorTarea = completados
            .GroupBy(x => x.TareaId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var items = new List<MisTareasItemDto>();

        foreach (var tarea in tareas)
        {
            var item = new MisTareasItemDto
            {
                TareaId = tarea.Id,
                Nombre = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                InstruccionesUrl = tarea.InstruccionesUrl,
                ImporteRecompensa = tarea.ImporteRecompensa,
                PuntosRecompensa = tarea.PuntosRecompensa,
                EsRepetible = tarea.EsRepetible,
                MaxRepeticiones = tarea.MaxRepeticiones,
                Orden = tarea.Orden
            };

            if (TipoEventoPromoNombres.TryGetValue(tarea.TipoEventoPromoId, out var eventoNombre))
                item.TipoEventoPromoNombre = eventoNombre;
            if (tarea.TipoRewardId.HasValue && TipoRewardNombres.TryGetValue(tarea.TipoRewardId.Value, out var rewardNombre))
                item.TipoRewardNombre = rewardNombre;
            if (tarea.MonedaId.HasValue && MonedaNombres.TryGetValue(tarea.MonedaId.Value, out var monedaNombre))
                item.MonedaNombre = monedaNombre;

            if (completadosPorTarea.TryGetValue(tarea.Id, out var registros) && registros.Count > 0)
            {
                var ultimoRegistro = registros.OrderByDescending(x => x.FechaCreacion).First();
                var vecesCompletada = registros.Count(x => x.EstadoTareaId != 1);
                var fechaPrimera = registros.Where(x => x.FechaCompletado.HasValue).Min(x => x.FechaCompletado);
                var fechaUltima = registros.Where(x => x.FechaCompletado.HasValue).Max(x => x.FechaCompletado);

                EstadoTareaNombres.TryGetValue(ultimoRegistro.EstadoTareaId, out var estadoNombre);

                item.MiEstado = new MiEstadoTareaDto
                {
                    TareaPromotorId = ultimoRegistro.Id,
                    EstadoTareaId = ultimoRegistro.EstadoTareaId,
                    EstadoTareaNombre = estadoNombre ?? "Desconocido",
                    VecesCompletada = vecesCompletada,
                    FechaPrimeraCompletada = fechaPrimera,
                    FechaUltimaCompletada = fechaUltima,
                    UrlPruebaCompletado = ultimoRegistro.UrlPruebaCompletado,
                    ComentarioValidacion = ultimoRegistro.ComentarioValidacion
                };
            }

            items.Add(item);
        }

        return new MisTareasResponseDto
        {
            ProgramaId = programaId.Value,
            ProgramaTitulo = programa?.Titulo ?? string.Empty,
            Items = items
        };
    }

    // Orchestration: POST completar
    public async Task<(CompletarTareaResponseDto Dto, int VecesCompletada)> CompletarTareaAsync(
        PromoTareaPromotor registro, bool esActualizacion, CancellationToken ct)
    {
        if (esActualizacion)
        {
            await _tareaPromotorRepository.UpdateAsync(registro, ct);
        }
        else
        {
            await _tareaPromotorRepository.AddAsync(registro, ct);
        }

        var vecesCompletada = await _tareaPromotorRepository.CountCompletadosActivosAsync(
            registro.TareaId, registro.ProgramaPromotorId, ct);

        var dto = new CompletarTareaResponseDto
        {
            TareaPromotorId = registro.Id,
            EstadoTareaId = registro.EstadoTareaId,
            EstadoTareaNombre = "Completada",
            VecesCompletada = vecesCompletada,
            FechaUltimaCompletada = registro.FechaCompletado
        };

        return (dto, vecesCompletada);
    }

    // Orchestration: GET tareas-pendientes
    public async Task<(IReadOnlyList<TareaPendienteItemDto> Items, int TotalCount)> GetTareasPendientesAsync(
        PromoProgramaId programaId, int page, int pageSize, CancellationToken ct)
    {
        var (pendientes, totalCount) = await _tareaPromotorRepository.GetPendientesPagedAsync(
            programaId, page, pageSize, ct);

        // Calculate vecesCompletada for each unique (TareaId, ProgramaPromotorId) pair in this page
        var pares = pendientes
            .Select(x => new { x.TareaId, x.ProgramaPromotorId })
            .Distinct()
            .ToList();

        var conteos = new Dictionary<(Guid TareaId, Guid ProgramaPromotorId), int>();
        foreach (var par in pares)
        {
            var count = await _context.TareaPromotores
                .CountAsync(x => x.TareaId == par.TareaId
                    && x.ProgramaPromotorId == par.ProgramaPromotorId, ct);
            conteos[(par.TareaId, par.ProgramaPromotorId)] = count;
        }

        var items = pendientes.Select(p =>
        {
            conteos.TryGetValue((p.TareaId, p.ProgramaPromotorId), out var veces);

            var item = new TareaPendienteItemDto
            {
                TareaPromotorId = p.Id,
                TareaId = p.TareaId,
                TareaNombre = p.Tarea?.Titulo ?? string.Empty,
                PromotorId = p.ProgramaPromotor?.PromotorId.Value ?? Guid.Empty,
                PromotorNombre = p.ProgramaPromotor?.Promotor?.NombrePublico ?? string.Empty,
                UrlPruebaCompletado = p.UrlPruebaCompletado,
                ComentarioPromotor = p.ComentarioPromotor,
                VecesCompletada = veces,
                FechaUltimaCompletada = p.FechaCompletado
            };

            if (p.ProgramaPromotor?.Promotor != null
                && TipoPromotorNombres.TryGetValue(p.ProgramaPromotor.Promotor.TipoPromotorId, out var tipoNombre))
            {
                item.PromotorTipoNombre = tipoNombre;
            }

            return item;
        }).ToList();

        return (items, totalCount);
    }

    // Orchestration: PATCH validar (transactional)
    public async Task<ValidarTareaResponseDto> ValidarTareaAsync(
        PromoTareaPromotor registro, PromoProgramaId programaId, string? comentario, CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            // Update PromoTareaPromotor state
            registro.EstadoTareaId = 3; // Validada
            registro.FechaValidado = DateTime.UtcNow;
            registro.ComentarioValidacion = comentario;

            _context.TareaPromotores.Update(registro);

            decimal? recompensaAcreditada = null;
            string? monedaNombre = null;
            int? puntosAcreditados = null;

            // If task has monetary reward, credit wallet
            if (registro.Tarea != null && registro.Tarea.ImporteRecompensa.HasValue)
            {
                var promotorId = registro.ProgramaPromotor!.PromotorId;
                var monedaId = registro.Tarea.MonedaId ?? 1; // Default EUR

                // Find or create wallet
                var wallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.PromotorId == promotorId && w.MonedaId == monedaId, ct);

                if (wallet == null)
                {
                    wallet = new PromotorWallet
                    {
                        Id = Guid.NewGuid(),
                        PromotorId = promotorId,
                        MonedaId = monedaId,
                        SaldoDisponible = 0,
                        SaldoPendiente = 0,
                        TotalGanado = 0,
                        TotalRetirado = 0,
                        FechaCreacion = DateTime.UtcNow
                    };
                    await _context.Wallets.AddAsync(wallet, ct);
                }

                // Create wallet transaction
                var walletTransaccion = new PromotorWalletTransaccion
                {
                    Id = Guid.NewGuid(),
                    WalletId = wallet.Id,
                    TipoRewardId = registro.Tarea.TipoRewardId,
                    EstadoTransaccionId = 1, // Pendiente de liquidacion
                    Importe = registro.Tarea.ImporteRecompensa.Value,
                    Concepto = $"Recompensa tarea: {registro.Tarea.Titulo}",
                    ReferenciaExterna = registro.Id.ToString(),
                    FechaCreacion = DateTime.UtcNow
                };
                await _context.WalletTransacciones.AddAsync(walletTransaccion, ct);

                // Update wallet balances
                wallet.SaldoPendiente += registro.Tarea.ImporteRecompensa.Value;
                wallet.TotalGanado += registro.Tarea.ImporteRecompensa.Value;
                wallet.FechaActualizacion = DateTime.UtcNow;

                recompensaAcreditada = registro.Tarea.ImporteRecompensa.Value;
                if (MonedaNombres.TryGetValue(monedaId, out var mNombre))
                    monedaNombre = mNombre;
            }

            if (registro.Tarea != null && registro.Tarea.PuntosRecompensa.HasValue)
            {
                puntosAcreditados = registro.Tarea.PuntosRecompensa.Value;
            }

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return new ValidarTareaResponseDto
            {
                TareaPromotorId = registro.Id,
                EstadoTareaId = 3,
                EstadoTareaNombre = "Validada",
                RecompensaAcreditada = recompensaAcreditada,
                MonedaNombre = monedaNombre,
                PuntosAcreditados = puntosAcreditados
            };
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    // Orchestration: PATCH rechazar
    public async Task RechazarTareaAsync(PromoTareaPromotor registro, string comentario, CancellationToken ct)
    {
        registro.EstadoTareaId = 4; // Rechazada
        registro.FechaValidado = DateTime.UtcNow;
        registro.ComentarioValidacion = comentario;

        await _tareaPromotorRepository.UpdateAsync(registro, ct);
    }
}
