using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Interfaces;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.Crowdpromotion.Infra.Context;

namespace WePlayRises.Crowdpromotion.Infra.Repositories;

public class PromoEventoRepository : IPromoEventoRepository
{
    private readonly CrowdpromotionContext _context;

    public PromoEventoRepository(CrowdpromotionContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Guid> AddAsync(PromoEvento entity, CancellationToken ct)
    {
        await _context.Eventos.AddAsync(entity, ct);
        // NOTE: Does NOT call SaveChanges - coordinated by Service for transactions
        return entity.Id;
    }

    public async Task<PromoEventoMetricasRaw> GetMetricasProgramaAsync(
        PromoProgramaId programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct)
    {
        var result = await _context.Eventos
            .AsNoTracking()
            .Where(e => e.ProgramaId == programaId
                && e.FechaCreacion >= fechaDesde
                && e.FechaCreacion <= fechaHasta)
            .GroupBy(e => 1)
            .Select(g => new PromoEventoMetricasRaw(
                g.Count(e => e.TipoEventoId == 1),
                g.Count(e => e.TipoEventoId == 2),
                g.Count(e => e.TipoEventoId == 3),
                g.Count(e => e.TipoEventoId == 4),
                g.Where(e => e.TipoEventoId == 4 && e.ImporteAsociado.HasValue)
                    .Sum(e => e.ImporteAsociado ?? 0)))
            .FirstOrDefaultAsync(ct);

        return result ?? new PromoEventoMetricasRaw(0, 0, 0, 0, 0);
    }

    public async Task<IReadOnlyList<PromoEventoRankingRaw>> GetRankingPromotoresAsync(
        PromoProgramaId programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct)
    {
        return await _context.Eventos
            .AsNoTracking()
            .Where(e => e.ProgramaId == programaId
                && e.PromoProgramaPromotorId != null
                && e.FechaCreacion >= fechaDesde
                && e.FechaCreacion <= fechaHasta)
            .GroupBy(e => e.PromoProgramaPromotorId!.Value)
            .Select(g => new PromoEventoRankingRaw(
                g.Key,
                g.Count(e => e.TipoEventoId == 1),
                g.Count(e => e.TipoEventoId == 2),
                g.Count(e => e.TipoEventoId == 3),
                g.Count(e => e.TipoEventoId == 4),
                g.Where(e => e.TipoEventoId == 4 && e.ImporteAsociado.HasValue)
                    .Sum(e => e.ImporteAsociado ?? 0)))
            .OrderByDescending(r => r.Conversiones)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PromoEventoPorDiaRaw>> GetEventosPorDiaAsync(
        PromoProgramaId programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct)
    {
        return await _context.Eventos
            .AsNoTracking()
            .Where(e => e.ProgramaId == programaId
                && e.FechaCreacion >= fechaDesde
                && e.FechaCreacion <= fechaHasta)
            .GroupBy(e => e.FechaCreacion.Date)
            .OrderBy(g => g.Key)
            .Select(g => new PromoEventoPorDiaRaw(
                DateOnly.FromDateTime(g.Key),
                g.Count(e => e.TipoEventoId == 1),
                g.Count(e => e.TipoEventoId == 2),
                g.Count(e => e.TipoEventoId == 3),
                g.Count(e => e.TipoEventoId == 4)))
            .ToListAsync(ct);
    }

    public async Task<PromoEventoMetricasPromotorRaw> GetMetricasPromotorAsync(
        PromotorId promotorId, Guid? programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct)
    {
        var query = _context.Eventos
            .AsNoTracking()
            .Where(e => e.PromotorId == promotorId
                && e.FechaCreacion >= fechaDesde
                && e.FechaCreacion <= fechaHasta);

        if (programaId.HasValue)
        {
            var promoProgramaId = new PromoProgramaId(programaId.Value);
            query = query.Where(e => e.ProgramaId == promoProgramaId);
        }

        var result = await query
            .GroupBy(e => 1)
            .Select(g => new PromoEventoMetricasPromotorRaw(
                g.Count(e => e.TipoEventoId == 1),
                g.Count(e => e.TipoEventoId == 2),
                g.Count(e => e.TipoEventoId == 3),
                g.Count(e => e.TipoEventoId == 4),
                g.Where(e => e.TipoEventoId == 4 && e.ImporteAsociado.HasValue)
                    .Sum(e => e.ImporteAsociado ?? 0)))
            .FirstOrDefaultAsync(ct);

        return result ?? new PromoEventoMetricasPromotorRaw(0, 0, 0, 0, 0);
    }

    public async Task<IReadOnlyList<PromoEvento>> GetEventosRecientesByPromotorAsync(
        PromotorId promotorId, Guid? programaId, int maxItems, CancellationToken ct)
    {
        var query = _context.Eventos
            .AsNoTracking()
            .Where(e => e.PromotorId == promotorId && e.TipoEventoId == 4);

        if (programaId.HasValue)
        {
            var promoProgramaId = new PromoProgramaId(programaId.Value);
            query = query.Where(e => e.ProgramaId == promoProgramaId);
        }

        return await query
            .OrderByDescending(e => e.FechaCreacion)
            .Take(maxItems)
            .ToListAsync(ct);
    }
}
