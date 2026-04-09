using Microsoft.EntityFrameworkCore;
using WePlayRises.Crowdpromotion.Domain.Interfaces;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.Crowdpromotion.Infra.Context;

namespace WePlayRises.Crowdpromotion.Infra.Repositories;

public class PromotorWalletTransaccionRepository : IPromotorWalletTransaccionRepository
{
    private readonly CrowdpromotionContext _context;

    public PromotorWalletTransaccionRepository(CrowdpromotionContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<(IReadOnlyList<PromotorWalletTransaccion> Items, int TotalCount)> GetPagedByWalletIdAsync(
        Guid walletId, bool? esCredito, int? estadoTransaccionId,
        DateTime? fechaDesde, DateTime? fechaHasta,
        int page, int pageSize, CancellationToken ct)
    {
        var query = _context.WalletTransacciones
            .AsNoTracking()
            .Where(t => t.WalletId == walletId);

        if (esCredito.HasValue)
            query = query.Where(t => t.EsCredito == esCredito.Value);

        if (estadoTransaccionId.HasValue)
            query = query.Where(t => t.EstadoTransaccionId == estadoTransaccionId.Value);

        if (fechaDesde.HasValue)
            query = query.Where(t => t.FechaCreacion >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(t => t.FechaCreacion <= fechaHasta.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(t => t.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task AddAsync(PromotorWalletTransaccion entity, CancellationToken ct)
    {
        await _context.WalletTransacciones.AddAsync(entity, ct);
    }

    public async Task<bool> HasPendienteByWalletIdAsync(Guid walletId, CancellationToken ct)
    {
        return await _context.WalletTransacciones
            .AsNoTracking()
            .AnyAsync(t => t.WalletId == walletId && !t.EsCredito && t.EstadoTransaccionId == 1, ct);
    }
}
