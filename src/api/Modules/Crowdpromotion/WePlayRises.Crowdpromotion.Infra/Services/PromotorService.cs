using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Interfaces;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.Crowdpromotion.Infra.Context;

namespace WePlayRises.Crowdpromotion.Infra.Services;

public class PromotorService : IPromotorService
{
    private readonly IPromotorRepository _promotorRepository;
    private readonly IPromotorWalletRepository _walletRepository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<PromotorService> _logger;
    private readonly CrowdpromotionContext _context;

    private static readonly HashSet<int> ValidTipoPromotorIds = new() { 1, 2, 3, 4 };

    public PromotorService(
        IPromotorRepository promotorRepository,
        IPromotorWalletRepository walletRepository,
        IRequestCacheService requestCache,
        ILogger<PromotorService> logger,
        CrowdpromotionContext context)
    {
        _promotorRepository = promotorRepository ?? throw new ArgumentNullException(nameof(promotorRepository));
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Promotor?> GetByIdAsync(PromotorId id, CancellationToken ct)
    {
        var cacheKey = $"promotor:{id.Value}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _promotorRepository.GetByIdAsync(id, ct));
    }

    public async Task<Promotor?> GetByUserIdAsync(string userId, CancellationToken ct)
    {
        var cacheKey = $"promotor:userid:{userId}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _promotorRepository.GetByUserIdAsync(userId, ct));
    }

    public Task<bool> TipoPromotorExistsAsync(int tipoPromotorId, CancellationToken ct)
    {
        return Task.FromResult(ValidTipoPromotorIds.Contains(tipoPromotorId));
    }

    public async Task<PromotorId> CreateWithWalletAsync(Promotor promotor, CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            promotor.FechaCreacion = DateTime.UtcNow;
            promotor.EsActivo = true;

            var promotorId = await _promotorRepository.AddAsync(promotor, ct);

            var wallet = new PromotorWallet
            {
                Id = Guid.NewGuid(),
                PromotorId = promotorId,
                MonedaId = 1,
                SaldoDisponible = 0m,
                SaldoPendiente = 0m,
                TotalGanado = 0m,
                TotalRetirado = 0m,
                FechaCreacion = DateTime.UtcNow
            };

            await _walletRepository.AddAsync(wallet, ct);

            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "Promotor {PromotorId} created for UserId {UserId} with EUR wallet",
                promotorId.Value, promotor.UserId);

            return promotorId;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task UpdateAsync(Promotor promotor, CancellationToken ct)
    {
        promotor.FechaActualizacion = DateTime.UtcNow;
        await _promotorRepository.UpdateAsync(promotor, ct);
        _logger.LogInformation("Promotor {PromotorId} updated", promotor.Id.Value);
    }

    public async Task<int> DesactivarWithProgramasAsync(PromotorId promotorId, CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            var promotor = await _promotorRepository.GetByIdAsync(promotorId, ct);
            if (promotor == null)
                throw new InvalidOperationException($"Promotor {promotorId.Value} not found");

            promotor.EsActivo = false;
            promotor.FechaActualizacion = DateTime.UtcNow;

            var activeProgramas = await _promotorRepository.GetActiveProgramasAsync(promotorId, ct);
            foreach (var programa in activeProgramas)
            {
                programa.EsActivo = false;
                programa.FechaBaja = DateTime.UtcNow;
            }

            await _promotorRepository.UpdateAsync(promotor, ct);

            if (activeProgramas.Count > 0)
            {
                _context.ProgramaPromotores.UpdateRange(activeProgramas);
                await _context.SaveChangesAsync(ct);
            }

            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "Promotor {PromotorId} deactivated. Programs deactivated: {Count}",
                promotorId.Value, activeProgramas.Count);

            return activeProgramas.Count;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<int> GetProgramasActivosCountAsync(PromotorId promotorId, CancellationToken ct)
    {
        var programas = await _promotorRepository.GetActiveProgramasAsync(promotorId, ct);
        return programas.Count;
    }

    public async Task<PromotorWallet?> GetWalletEurAsync(PromotorId promotorId, CancellationToken ct)
    {
        var cacheKey = $"promotorwallet:{promotorId.Value}:moneda:1";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _walletRepository.GetByPromotorIdAndMonedaAsync(promotorId, 1, ct));
    }
}
