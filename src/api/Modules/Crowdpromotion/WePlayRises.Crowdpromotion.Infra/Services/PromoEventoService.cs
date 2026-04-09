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

public class PromoEventoService : IPromoEventoService
{
    private readonly IPromoEventoRepository _eventoRepository;
    private readonly IPromoProgramaPromotorRepository _programaPromotorRepository;
    private readonly IPromotorWalletRepository _walletRepository;
    private readonly IPromotorRepository _promotorRepository;
    private readonly IRequestCacheService _requestCache;
    private readonly CrowdpromotionContext _context;
    private readonly ILogger<PromoEventoService> _logger;

    public PromoEventoService(
        IPromoEventoRepository eventoRepository,
        IPromoProgramaPromotorRepository programaPromotorRepository,
        IPromotorWalletRepository walletRepository,
        IPromotorRepository promotorRepository,
        IRequestCacheService requestCache,
        CrowdpromotionContext context,
        ILogger<PromoEventoService> logger)
    {
        _eventoRepository = eventoRepository ?? throw new ArgumentNullException(nameof(eventoRepository));
        _programaPromotorRepository = programaPromotorRepository ?? throw new ArgumentNullException(nameof(programaPromotorRepository));
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _promotorRepository = promotorRepository ?? throw new ArgumentNullException(nameof(promotorRepository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PromoProgramaPromotor?> ResolverPromotorPorCodigoReferidoAsync(
        string codigoReferido, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"tracking:ref:{codigoReferido}",
            async () => await _programaPromotorRepository.GetByCodigoReferidoActivoAsync(codigoReferido, ct));
    }

    public async Task<Guid> RegistrarEventoAsync(PromoEvento evento, CancellationToken ct)
    {
        // Resolve CodigoReferido to ProgramaId, PromotorId, PromoProgramaPromotorId
        if (!string.IsNullOrEmpty(evento.CodigoReferido))
        {
            var inscripcion = await ResolverPromotorPorCodigoReferidoAsync(evento.CodigoReferido, ct);
            if (inscripcion != null)
            {
                evento.ProgramaId = inscripcion.ProgramaId;
                evento.PromotorId = inscripcion.PromotorId;
                evento.PromoProgramaPromotorId = inscripcion.Id;
            }
            // If null: FA-01 - anonymous tracking, ProgramaId/PromotorId remain null
        }

        await _eventoRepository.AddAsync(evento, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("PromoEvento {EventoId} tipo {TipoEventoId} registrado",
            evento.Id, evento.TipoEventoId);

        return evento.Id;
    }

    public async Task<RegistrarConversionResult> RegistrarConversionAsync(
        PromoEvento evento, CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            // 1. Resolve CodigoReferido
            PromoProgramaPromotor? inscripcion = null;
            PromoPrograma? programa = null;

            if (!string.IsNullOrEmpty(evento.CodigoReferido))
            {
                inscripcion = await ResolverPromotorPorCodigoReferidoAsync(evento.CodigoReferido, ct);
                if (inscripcion != null)
                {
                    evento.ProgramaId = inscripcion.ProgramaId;
                    evento.PromotorId = inscripcion.PromotorId;
                    evento.PromoProgramaPromotorId = inscripcion.Id;
                    programa = inscripcion.Programa;
                }
            }

            // 2. Insert PromoEvento
            await _eventoRepository.AddAsync(evento, ct);

            // 3. Calculate and accredit commission
            decimal comisionCalculada = 0;
            Guid? walletTransaccionId = null;
            bool comisionAcreditada = false;
            string? monedaNombre = null;

            if (programa != null && programa.EsActivo
                && inscripcion != null && inscripcion.EsAprobado
                && !inscripcion.EsBloqueado && !inscripcion.FechaBaja.HasValue)
            {
                // Calculate commission
                var comisionPorcentaje = (programa.ImporteComisionPorcentaje ?? 0) > 0 && evento.ImporteAsociado.HasValue
                    ? evento.ImporteAsociado.Value * programa.ImporteComisionPorcentaje!.Value / 100m
                    : 0m;

                var comisionFija = programa.ImporteComisionFija ?? 0m;

                if (comisionPorcentaje > 0 && comisionFija > 0)
                    comisionCalculada = Math.Max(comisionPorcentaje, comisionFija);
                else if (comisionPorcentaje > 0)
                    comisionCalculada = comisionPorcentaje;
                else if (comisionFija > 0)
                    comisionCalculada = comisionFija;

                if (comisionCalculada > 0)
                {
                    var monedaId = programa.MonedaId ?? 1; // fallback to EUR (1)

                    // Find or create wallet
                    var wallet = await _walletRepository.GetByPromotorIdAndMonedaAsync(
                        inscripcion.PromotorId, monedaId, ct);

                    if (wallet == null)
                    {
                        wallet = new PromotorWallet
                        {
                            Id = Guid.NewGuid(),
                            PromotorId = inscripcion.PromotorId,
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
                        EsCredito = true,
                        EstadoTransaccionId = 1, // Pendiente
                        Importe = comisionCalculada,
                        Concepto = "Comision backing referido",
                        ReferenciaExterna = evento.Id.ToString(),
                        PromoEventoId = evento.Id,
                        Descripcion = "Comision por backing referido",
                        FechaCreacion = DateTime.UtcNow
                    };
                    await _context.WalletTransacciones.AddAsync(walletTransaccion, ct);

                    // Update wallet balance
                    wallet.SaldoPendiente += comisionCalculada;
                    wallet.TotalGanado += comisionCalculada;
                    wallet.FechaActualizacion = DateTime.UtcNow;

                    walletTransaccionId = walletTransaccion.Id;
                    comisionAcreditada = true;
                }
            }

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "Conversion registrada. EventoId: {EventoId}, Comision: {Comision}, Acreditada: {Acreditada}",
                evento.Id, comisionCalculada, comisionAcreditada);

            return new RegistrarConversionResult(
                evento.Id, comisionCalculada, walletTransaccionId, comisionAcreditada, monedaNombre);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogError(ex, "Error en RegistrarConversionAsync. Rolling back transaction");
            throw;
        }
    }

    public async Task<ProgramaMetricasResponseDto> GetProgramaMetricasAsync(
        Guid programaId, DateOnly fechaDesde, DateOnly fechaHasta, CancellationToken ct)
    {
        var promoProgramaId = new PromoProgramaId(programaId);
        var fechaDesdeUtc = fechaDesde.ToDateTime(TimeOnly.MinValue);
        var fechaHastaUtc = fechaHasta.ToDateTime(TimeOnly.MaxValue);

        // Get program info
        var programa = await _context.Programas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == promoProgramaId, ct);

        // Get KPIs
        var kpisRaw = await _eventoRepository.GetMetricasProgramaAsync(
            promoProgramaId, fechaDesdeUtc, fechaHastaUtc, ct);

        // Get ranking
        var rankingRaw = await _eventoRepository.GetRankingPromotoresAsync(
            promoProgramaId, fechaDesdeUtc, fechaHastaUtc, ct);

        // Get events per day
        var porDiaRaw = await _eventoRepository.GetEventosPorDiaAsync(
            promoProgramaId, fechaDesdeUtc, fechaHastaUtc, ct);

        // Calculate comisiones totales from wallet transactions
        var comisionesTotales = await _context.WalletTransacciones
            .AsNoTracking()
            .Where(wt => _context.Eventos
                .Where(e => e.ProgramaId == promoProgramaId
                    && e.TipoEventoId == 4
                    && e.FechaCreacion >= fechaDesdeUtc
                    && e.FechaCreacion <= fechaHastaUtc)
                .Select(e => e.Id.ToString())
                .Contains(wt.ReferenciaExterna))
            .SumAsync(wt => wt.Importe, ct);

        // Enrich ranking with promotor data
        var rankingDtos = new List<RankingPromotorItemDto>();
        if (rankingRaw.Count > 0)
        {
            var inscripcionIds = rankingRaw.Select(r => r.PromoProgramaPromotorId).ToList();
            var inscripciones = await _context.ProgramaPromotores
                .AsNoTracking()
                .Include(ppp => ppp.Promotor)
                .Where(ppp => inscripcionIds.Contains(ppp.Id))
                .ToListAsync(ct);

            var inscripcionMap = inscripciones.ToDictionary(i => i.Id);

            foreach (var raw in rankingRaw)
            {
                var inscripcion = inscripcionMap.GetValueOrDefault(raw.PromoProgramaPromotorId);
                rankingDtos.Add(new RankingPromotorItemDto
                {
                    PromotorId = inscripcion?.PromotorId.Value ?? Guid.Empty,
                    PromotorNombre = inscripcion?.Promotor?.NombrePublico ?? "Desconocido",
                    TipoPromotorNombre = null, // Master data resolution simplified for MVP
                    Clicks = raw.Clicks,
                    PageViews = raw.PageViews,
                    Signups = raw.Signups,
                    Conversiones = raw.Conversiones,
                    ValorGenerado = raw.ValorGenerado,
                    ComisionAcumulada = 0 // Simplified for MVP
                });
            }
        }

        var tasaConversion = kpisRaw.TotalClicks > 0
            ? Math.Round((decimal)kpisRaw.TotalConversiones / kpisRaw.TotalClicks * 100, 2)
            : 0;

        return new ProgramaMetricasResponseDto
        {
            ProgramaId = programaId,
            ProgramaTitulo = programa?.Titulo ?? "Programa",
            FechaDesde = fechaDesde.ToString("yyyy-MM-dd"),
            FechaHasta = fechaHasta.ToString("yyyy-MM-dd"),
            Kpis = new ProgramaMetricasKpisDto
            {
                TotalClicks = kpisRaw.TotalClicks,
                TotalPageViews = kpisRaw.TotalPageViews,
                TotalSignups = kpisRaw.TotalSignups,
                TotalConversiones = kpisRaw.TotalConversiones,
                ValorTotalGenerado = kpisRaw.ValorTotalGenerado,
                MonedaNombre = null, // Simplified for MVP
                TasaConversion = tasaConversion,
                ComisionesTotales = comisionesTotales
            },
            RankingPromotores = rankingDtos,
            EventosPorDia = porDiaRaw.Select(d => new EventosPorDiaItemDto
            {
                Fecha = d.Fecha.ToString("yyyy-MM-dd"),
                Clicks = d.Clicks,
                PageViews = d.PageViews,
                Signups = d.Signups,
                Conversiones = d.Conversiones
            }).ToList()
        };
    }

    public async Task<PromotorMetricasResponseDto> GetPromotorMetricasAsync(
        PromotorId promotorId, Guid? programaId, DateOnly fechaDesde, DateOnly fechaHasta, CancellationToken ct)
    {
        var fechaDesdeUtc = fechaDesde.ToDateTime(TimeOnly.MinValue);
        var fechaHastaUtc = fechaHasta.ToDateTime(TimeOnly.MaxValue);

        // Get promotor info
        var promotor = await _requestCache.GetOrAddAsync(
            $"promotor:{promotorId.Value}",
            async () => await _promotorRepository.GetByIdAsync(promotorId, ct));

        // Get KPIs
        var kpisRaw = await _eventoRepository.GetMetricasPromotorAsync(
            promotorId, programaId, fechaDesdeUtc, fechaHastaUtc, ct);

        // Get recent events (Backing only, max 20)
        var eventosRecientes = await _eventoRepository.GetEventosRecientesByPromotorAsync(
            promotorId, programaId, 20, ct);

        // Get accumulated commission
        var wallets = await _context.Wallets
            .AsNoTracking()
            .Where(w => w.PromotorId == promotorId)
            .Select(w => w.Id)
            .ToListAsync(ct);

        var miComisionAcumulada = wallets.Count > 0
            ? await _context.WalletTransacciones
                .AsNoTracking()
                .Where(wt => wallets.Contains(wt.WalletId)
                    && wt.FechaCreacion >= fechaDesdeUtc
                    && wt.FechaCreacion <= fechaHastaUtc)
                .SumAsync(wt => wt.Importe, ct)
            : 0;

        // Get program title if filtered
        string? programaTitulo = null;
        if (programaId.HasValue)
        {
            var promoProgramaId = new PromoProgramaId(programaId.Value);
            var programa = await _context.Programas
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == promoProgramaId, ct);
            programaTitulo = programa?.Titulo;
        }

        var miTasaConversion = kpisRaw.MisClicks > 0
            ? Math.Round((decimal)kpisRaw.MisConversiones / kpisRaw.MisClicks * 100, 2)
            : 0;

        return new PromotorMetricasResponseDto
        {
            PromotorId = promotorId.Value,
            PromotorNombre = promotor?.NombrePublico ?? "Promotor",
            ProgramaId = programaId,
            ProgramaTitulo = programaTitulo,
            FechaDesde = fechaDesde.ToString("yyyy-MM-dd"),
            FechaHasta = fechaHasta.ToString("yyyy-MM-dd"),
            Kpis = new PromotorMetricasKpisDto
            {
                MisClicks = kpisRaw.MisClicks,
                MisPageViews = kpisRaw.MisPageViews,
                MisSignups = kpisRaw.MisSignups,
                MisConversiones = kpisRaw.MisConversiones,
                MiValorGenerado = kpisRaw.MiValorGenerado,
                MiComisionAcumulada = miComisionAcumulada,
                MonedaNombre = null, // Simplified for MVP
                MiTasaConversion = miTasaConversion
            },
            EventosRecientes = eventosRecientes.Select(e => new EventoRecienteDto
            {
                Id = e.Id,
                TipoEventoId = e.TipoEventoId,
                TipoEventoNombre = "Backing",
                ValorMonetario = e.ImporteAsociado ?? 0,
                ComisionGenerada = null, // Simplified for MVP
                MonedaNombre = null, // Simplified for MVP
                FechaEvento = e.FechaCreacion
            }).ToList()
        };
    }

    public async Task<Promotor?> GetPromotorByUserIdAsync(string userId, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"promotor:user:{userId}",
            async () => await _promotorRepository.GetByUserIdAsync(userId, ct));
    }

    public async Task<PromoPrograma?> GetProgramaByIdAsync(Guid programaId, CancellationToken ct)
    {
        var promoProgramaId = new PromoProgramaId(programaId);
        return await _requestCache.GetOrAddAsync(
            $"programa:{programaId}",
            async () => await _context.Programas
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == promoProgramaId, ct));
    }
}
