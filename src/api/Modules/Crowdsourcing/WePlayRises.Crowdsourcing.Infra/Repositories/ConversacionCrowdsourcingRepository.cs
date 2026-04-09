using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class ConversacionCrowdsourcingRepository : IConversacionCrowdsourcingRepository
{
    private readonly CrowdsourcingContext _context;

    public ConversacionCrowdsourcingRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Guid> AddAsync(ConversacionCrowdsourcing entity, CancellationToken ct)
    {
        await _context.Conversaciones.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<ConversacionCrowdsourcing?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Conversaciones
            .Include(x => x.Necesidad)
            .Include(x => x.Acuerdo)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<bool> ExisteConversacionParaContextoAsync(
        string userIdCreador, string userIdDestinatario,
        Guid? necesidadId, Guid? acuerdoId, CancellationToken ct)
    {
        return await _context.Conversaciones.AnyAsync(x =>
            ((x.UserIdCreador == userIdCreador && x.UserIdDestinatario == userIdDestinatario) ||
             (x.UserIdCreador == userIdDestinatario && x.UserIdDestinatario == userIdCreador)) &&
            (necesidadId.HasValue
                ? x.NecesidadId == new NecesidadCrowdsourcingId(necesidadId.Value)
                : x.AcuerdoId == new AcuerdoCrowdsourcingId(acuerdoId!.Value)),
            ct);
    }

    public async Task<(IReadOnlyList<ConversacionCrowdsourcing> Items, int TotalCount, int TotalNoLeidos)>
        GetConversacionesByUserIdAsync(
            string userId, string? filtroContexto, int page, int pageSize, CancellationToken ct)
    {
        var baseQuery = _context.Conversaciones
            .AsNoTracking()
            .Include(x => x.Necesidad)
            .Include(x => x.Acuerdo)
            .Include(x => x.Mensajes)
            .Where(x => x.UserIdCreador == userId || x.UserIdDestinatario == userId);

        // Apply context filter
        if (!string.IsNullOrEmpty(filtroContexto) && filtroContexto != "todas")
        {
            baseQuery = filtroContexto switch
            {
                "necesidades" => baseQuery.Where(x => x.NecesidadId != null),
                "acuerdos" => baseQuery.Where(x => x.AcuerdoId != null),
                _ => baseQuery
            };
        }

        var totalCount = await baseQuery.CountAsync(ct);

        // Calculate total unread across ALL conversations (no filter, no pagination)
        var totalNoLeidos = await _context.Mensajes
            .CountAsync(m =>
                _context.Conversaciones.Any(c =>
                    c.Id == m.ConversacionId &&
                    (c.UserIdCreador == userId || c.UserIdDestinatario == userId)) &&
                m.UserIdRemitente != userId &&
                !m.Leido,
                ct);

        // Order: conversations with messages first (desc by FechaUltimoMensaje), then by FechaCreacion desc
        var items = await baseQuery
            .OrderByDescending(x => x.FechaUltimoMensaje.HasValue ? 1 : 0)
            .ThenByDescending(x => x.FechaUltimoMensaje ?? x.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount, totalNoLeidos);
    }

    public async Task UpdateFechaUltimoMensajeAsync(
        Guid conversacionId, DateTime fechaUltimoMensaje, CancellationToken ct)
    {
        await _context.Conversaciones
            .Where(x => x.Id == conversacionId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(x => x.FechaUltimoMensaje, fechaUltimoMensaje),
                ct);
    }

    public async Task<int> GetTotalNoLeidosByUserIdAsync(string userId, CancellationToken ct)
    {
        return await _context.Mensajes
            .Join(_context.Conversaciones,
                m => m.ConversacionId,
                c => c.Id,
                (m, c) => new { m, c })
            .CountAsync(x =>
                (x.c.UserIdCreador == userId || x.c.UserIdDestinatario == userId) &&
                x.m.UserIdRemitente != userId &&
                !x.m.Leido,
                ct);
    }
}
