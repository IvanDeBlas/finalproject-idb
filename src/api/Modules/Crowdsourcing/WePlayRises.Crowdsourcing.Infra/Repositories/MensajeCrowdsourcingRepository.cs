using Microsoft.EntityFrameworkCore;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class MensajeCrowdsourcingRepository : IMensajeCrowdsourcingRepository
{
    private readonly CrowdsourcingContext _context;

    public MensajeCrowdsourcingRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Guid> AddAsync(MensajeCrowdsourcing entity, CancellationToken ct)
    {
        await _context.Mensajes.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<(IReadOnlyList<MensajeCrowdsourcing> Items, int TotalCount)> GetByConversacionIdPaginatedAsync(
        Guid conversacionId, int page, int pageSize, CancellationToken ct)
    {
        var query = _context.Mensajes
            .AsNoTracking()
            .Where(x => x.ConversacionId == conversacionId);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<int> MarcarLeidosByConversacionAsync(
        Guid conversacionId, string userIdDestinatario, DateTime fechaLeido, CancellationToken ct)
    {
        var count = await _context.Mensajes
            .Where(x =>
                x.ConversacionId == conversacionId &&
                x.UserIdRemitente != userIdDestinatario &&
                !x.Leido)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Leido, true)
                .SetProperty(x => x.FechaLeido, fechaLeido),
                ct);

        return count;
    }
}
