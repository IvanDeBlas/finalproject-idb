using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class ValoracionCrowdsourcingRepository : IValoracionCrowdsourcingRepository
{
    private readonly CrowdsourcingContext _context;

    public ValoracionCrowdsourcingRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Guid> AddAsync(ValoracionCrowdsourcing entity, CancellationToken ct)
    {
        await _context.Valoraciones.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<bool> ExisteValoracionAsync(
        AcuerdoCrowdsourcingId acuerdoId, string userIdAutor, CancellationToken ct)
    {
        return await _context.Valoraciones
            .AsNoTracking()
            .AnyAsync(v => v.AcuerdoId == acuerdoId && v.UserIdAutor == userIdAutor, ct);
    }

    public async Task<ValoracionResumenData> GetResumenByUserIdAsync(string userId, CancellationToken ct)
    {
        var query = _context.Valoraciones
            .AsNoTracking()
            .Where(v => v.UserIdValorado == userId);

        var stats = await query
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Media = (decimal?)g.Average(v => (decimal)v.Puntuacion)
            })
            .FirstOrDefaultAsync(ct);

        var distribucionRaw = await query
            .GroupBy(v => v.Puntuacion)
            .Select(g => new { Puntuacion = (int)g.Key, Conteo = g.Count() })
            .ToListAsync(ct);

        var distribucion = Enumerable.Range(1, 5)
            .ToDictionary(
                estrella => estrella,
                estrella => distribucionRaw.FirstOrDefault(d => d.Puntuacion == estrella)?.Conteo ?? 0);

        return new ValoracionResumenData
        {
            PuntuacionMedia = stats?.Media.HasValue == true
                ? Math.Round(stats.Media.Value, 1, MidpointRounding.AwayFromZero)
                : null,
            TotalValoraciones = stats?.Total ?? 0,
            Distribucion = distribucion
        };
    }

    public async Task<(IReadOnlyList<ValoracionCrowdsourcing> Items, int TotalCount)> GetByUserIdPagedAsync(
        string userId, int page, int pageSize, CancellationToken ct)
    {
        var query = _context.Valoraciones
            .AsNoTracking()
            .Where(v => v.UserIdValorado == userId);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(v => v.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(v => v.Acuerdo)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
