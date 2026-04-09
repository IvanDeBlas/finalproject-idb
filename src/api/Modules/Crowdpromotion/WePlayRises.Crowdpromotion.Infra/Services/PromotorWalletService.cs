using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Interfaces;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.Crowdpromotion.Infra.Context;

namespace WePlayRises.Crowdpromotion.Infra.Services;

public class PromotorWalletService : IPromotorWalletService
{
    private readonly IPromotorWalletRepository _walletRepository;
    private readonly IPromotorWalletTransaccionRepository _transaccionRepository;
    private readonly IPromotorService _promotorService;
    private readonly CrowdpromotionContext _context;
    private readonly IConfiguration _configuration;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<PromotorWalletService> _logger;

    public PromotorWalletService(
        IPromotorWalletRepository walletRepository,
        IPromotorWalletTransaccionRepository transaccionRepository,
        IPromotorService promotorService,
        CrowdpromotionContext context,
        IConfiguration configuration,
        IRequestCacheService requestCache,
        ILogger<PromotorWalletService> logger)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _transaccionRepository = transaccionRepository ?? throw new ArgumentNullException(nameof(transaccionRepository));
        _promotorService = promotorService ?? throw new ArgumentNullException(nameof(promotorService));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PromotorWallet?> GetWalletEurByPromotorIdAsync(PromotorId promotorId, CancellationToken ct)
    {
        var cacheKey = $"promotorwallet:{promotorId.Value}:moneda:1";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _walletRepository.GetByPromotorIdAndMonedaAsync(promotorId, 1, ct));
    }

    public async Task<Promotor?> GetPromotorByUserIdAsync(string userId, CancellationToken ct)
    {
        return await _promotorService.GetByUserIdAsync(userId, ct);
    }

    public async Task<(PromotorWallet? Wallet, string? MonedaNombre)> GetWalletConMonedaAsync(
        PromotorId promotorId, CancellationToken ct)
    {
        var cacheKey = $"promotorwallet:{promotorId.Value}:moneda:1";
        var wallet = await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _walletRepository.GetByPromotorIdAndMonedaAsync(promotorId, 1, ct));

        if (wallet == null)
            return (null, null);

        return (wallet, "EUR");
    }

    public async Task<(IReadOnlyList<PromotorWalletTransaccion> Items, int TotalCount)> GetTransaccionesPagedAsync(
        Guid walletId, bool? esCredito, int? estadoTransaccionId,
        DateTime? fechaDesde, DateTime? fechaHasta,
        int page, int pageSize, CancellationToken ct)
    {
        return await _transaccionRepository.GetPagedByWalletIdAsync(
            walletId, esCredito, estadoTransaccionId,
            fechaDesde, fechaHasta, page, pageSize, ct);
    }

    public async Task<(SolicitarCobroResult? Result, CobroError Error)> SolicitarCobroAsync(
        PromotorId promotorId, decimal importe, string? descripcion, CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            // 1. Get wallet with tracking for RowVersion
            var wallet = await _walletRepository.GetByPromotorIdAndMonedaForUpdateAsync(promotorId, 1, ct);
            if (wallet == null)
                return (null, CobroError.WalletNoEncontrado);

            // 2. Read min withdrawal from config
            var minimoRetiro = _configuration.GetValue<decimal>("Crowdpromotion:MinimoRetiro", 10.0m);

            // 3. Business validations
            if (wallet.SaldoDisponible < minimoRetiro)
                return (null, CobroError.SaldoBajoMinimo);

            if (importe > wallet.SaldoDisponible)
                return (null, CobroError.SaldoInsuficiente);

            if (await _transaccionRepository.HasPendienteByWalletIdAsync(wallet.Id, ct))
                return (null, CobroError.CobroConcurrente);

            // 4. Create debit transaction
            var transaccion = new PromotorWalletTransaccion
            {
                Id = Guid.NewGuid(),
                WalletId = wallet.Id,
                EsCredito = false,
                EstadoTransaccionId = 1,
                Importe = importe,
                Descripcion = descripcion,
                PromoEventoId = null,
                FechaCreacion = DateTime.UtcNow
            };
            await _transaccionRepository.AddAsync(transaccion, ct);

            // 5. Update wallet balance (RowVersion checked on SaveChanges)
            wallet.SaldoDisponible -= importe;
            wallet.TotalRetirado += importe;
            wallet.FechaActualizacion = DateTime.UtcNow;
            await _walletRepository.UpdateSaldoAsync(wallet, ct);

            // 6. Atomic commit
            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return (new SolicitarCobroResult(
                transaccion.Id,
                importe,
                "EUR",
                wallet.SaldoDisponible,
                transaccion.FechaCreacion), CobroError.None);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogWarning("Optimistic concurrency conflict on wallet for PromotorId {PromotorId}", promotorId.Value);
            return (null, CobroError.CobroConcurrente);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogError(ex, "Error in SolicitarCobroAsync for PromotorId {PromotorId}", promotorId.Value);
            throw;
        }
    }
}
