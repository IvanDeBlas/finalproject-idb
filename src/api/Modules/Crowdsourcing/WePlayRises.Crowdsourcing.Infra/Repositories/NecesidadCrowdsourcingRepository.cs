using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class NecesidadCrowdsourcingRepository : INecesidadCrowdsourcingRepository
{
    private readonly CrowdsourcingContext _context;

    public NecesidadCrowdsourcingRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<NecesidadCrowdsourcingId> AddAsync(NecesidadCrowdsourcing entity, CancellationToken ct)
    {
        await _context.Necesidades.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<List<NecesidadCrowdsourcingId>> AddManyAsync(List<NecesidadCrowdsourcing> entities, CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            await _context.Necesidades.AddRangeAsync(entities, ct);
            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return entities.Select(e => e.Id).ToList();
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<NecesidadCrowdsourcing?> GetByIdAsync(NecesidadCrowdsourcingId id, CancellationToken ct)
    {
        return await _context.Necesidades
            .FirstOrDefaultAsync(n => n.Id == id, ct);
    }

    public async Task<NecesidadCrowdsourcing?> GetByIdWithDetailsAsync(NecesidadCrowdsourcingId id, CancellationToken ct)
    {
        return await _context.Necesidades
            .Include(n => n.Propuestas)
            .FirstOrDefaultAsync(n => n.Id == id, ct);
    }

    public async Task<(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)> GetByArtistaIdPaginatedAsync(
        ArtistaId artistaId,
        int? estadoNecesidadId,
        string? search,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query = _context.Necesidades
            .AsNoTracking()
            .Where(n => n.ArtistaId == artistaId);

        if (estadoNecesidadId.HasValue)
        {
            query = query.Where(n => n.EstadoNecesidadId == estadoNecesidadId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(n =>
                n.Titulo.Contains(search) ||
                (n.Descripcion != null && n.Descripcion.Contains(search)));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(n => n.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task UpdateAsync(NecesidadCrowdsourcing entity, CancellationToken ct)
    {
        _context.Necesidades.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)> GetPublicasPaginatedAsync(
        NecesidadesPublicasFiltro filtro,
        ArtistaId? artistaDelUsuario,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var query = _context.Necesidades
            .AsNoTracking()
            .Include(n => n.Propuestas)
            .Where(n => n.EstadoNecesidadId == 1) // Abierta
            .Where(n => !n.FechaLimitePropuestas.HasValue || n.FechaLimitePropuestas.Value > now);

        if (artistaDelUsuario.HasValue)
        {
            query = query.Where(n => n.ArtistaId != artistaDelUsuario.Value);
        }

        if (filtro.TiposNecesidadIds != null && filtro.TiposNecesidadIds.Length > 0)
        {
            query = query.Where(n => filtro.TiposNecesidadIds.Contains(n.TipoNecesidadId));
        }

        if (filtro.ModalidadTrabajoId.HasValue)
        {
            query = query.Where(n => n.ModalidadTrabajoId == filtro.ModalidadTrabajoId.Value);
        }

        if (filtro.PresupuestoMin.HasValue)
        {
            query = query.Where(n => n.PresupuestoMax >= filtro.PresupuestoMin.Value);
        }

        if (filtro.PresupuestoMax.HasValue)
        {
            query = query.Where(n => n.PresupuestoMin <= filtro.PresupuestoMax.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Pais))
        {
            query = query.Where(n => n.UbicacionPais == filtro.Pais);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Search))
        {
            query = query.Where(n =>
                n.Titulo.Contains(filtro.Search) ||
                (n.Descripcion != null && n.Descripcion.Contains(filtro.Search)));
        }

        var totalCount = await query.CountAsync(ct);

        query = filtro.OrderBy switch
        {
            "mayor-presupuesto" => query.OrderByDescending(n => n.PresupuestoMax),
            "fecha-limite" => query.OrderBy(n => n.FechaLimitePropuestas),
            _ => query.OrderByDescending(n => n.FechaCreacion)
        };

        var items = await query
            .Skip((filtro.Page - 1) * filtro.PageSize)
            .Take(filtro.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<NecesidadCrowdsourcing?> GetPublicaByIdAsync(
        NecesidadCrowdsourcingId id,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        return await _context.Necesidades
            .AsNoTracking()
            .Include(n => n.Propuestas)
            .FirstOrDefaultAsync(n =>
                n.Id == id
                && n.EstadoNecesidadId == 1
                && (!n.FechaLimitePropuestas.HasValue || n.FechaLimitePropuestas.Value > now), ct);
    }
}
