using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class PropuestaCrowdsourcingRepository : IPropuestaCrowdsourcingRepository
{
    private readonly CrowdsourcingContext _context;

    public PropuestaCrowdsourcingRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IReadOnlyList<PropuestaCrowdsourcing>> GetPendientesByNecesidadIdAsync(
        NecesidadCrowdsourcingId necesidadId,
        CancellationToken ct)
    {
        return await _context.Propuestas
            .Where(p => p.NecesidadId == necesidadId && p.EstadoPropuestaId == EstadoPropuestaConstants.Pendiente)
            .ToListAsync(ct);
    }

    public async Task UpdateManyAsync(IEnumerable<PropuestaCrowdsourcing> propuestas, CancellationToken ct)
    {
        _context.Propuestas.UpdateRange(propuestas);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<PropuestaCrowdsourcing?> GetByIdAsync(PropuestaCrowdsourcingId id, CancellationToken ct)
    {
        return await _context.Propuestas
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<PropuestaCrowdsourcingId> AddAsync(PropuestaCrowdsourcing entity, CancellationToken ct)
    {
        await _context.Propuestas.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(PropuestaCrowdsourcing entity, CancellationToken ct)
    {
        _context.Propuestas.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExisteAsync(
        NecesidadCrowdsourcingId necesidadId,
        string userId,
        CancellationToken ct)
    {
        return await _context.Propuestas
            .AnyAsync(p =>
                p.NecesidadId == necesidadId
                && p.UserId == userId
                && p.EstadoPropuestaId != EstadoPropuestaConstants.Retirada, ct);
    }

    public async Task<(IReadOnlyList<PropuestaCrowdsourcing> Items, int TotalCount)> GetByUserIdPaginatedAsync(
        string userId,
        int? estadoPropuestaId,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query = _context.Propuestas
            .AsNoTracking()
            .Include(p => p.Necesidad)
            .Include(p => p.Acuerdos)
            .Where(p => p.UserId == userId);

        if (estadoPropuestaId.HasValue)
        {
            query = query.Where(p => p.EstadoPropuestaId == estadoPropuestaId.Value);
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
