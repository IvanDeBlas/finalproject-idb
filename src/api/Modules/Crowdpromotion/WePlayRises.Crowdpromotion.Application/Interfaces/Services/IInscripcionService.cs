using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Interfaces.Services;

public interface IInscripcionService
{
    // Identity resolution
    Task<Promotor?> GetPromotorByUserIdAsync(string userId, CancellationToken ct);
    Task<ArtistaId?> GetArtistaIdByUserIdAsync(string userId, CancellationToken ct);

    // PromoPrograma access
    Task<PromoPrograma?> GetProgramaByIdAsync(PromoProgramaId id, CancellationToken ct);
    Task<(IReadOnlyList<PromoPrograma> Items, int TotalCount)> GetProgramasActivosAsync(
        string? artistaNombre, int? tipoPromoId, int page, int pageSize, PromotorId promotorId, CancellationToken ct);

    // Inscripcion CRUD
    Task<PromoProgramaPromotor?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PromoProgramaPromotor?> GetByIdWithPromotorAsync(Guid id, CancellationToken ct);
    Task<PromoProgramaPromotor?> GetByPromotorYProgramaAsync(PromotorId promotorId, PromoProgramaId programaId, CancellationToken ct);
    Task<Guid> CreateAsync(PromoProgramaPromotor inscripcion, CancellationToken ct);
    Task AprobarAsync(PromoProgramaPromotor inscripcion, CancellationToken ct);
    Task RechazarAsync(Guid inscripcionId, CancellationToken ct);
    Task BloquearAsync(PromoProgramaPromotor inscripcion, CancellationToken ct);
    Task DarDeBajaAsync(PromoProgramaPromotor inscripcion, CancellationToken ct);

    // Queries
    Task<(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)> GetInscripcionesPorProgramaAsync(
        PromoProgramaId programaId, string? estado, int page, int pageSize, CancellationToken ct);
    Task<(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)> GetMisInscripcionesAsync(
        PromotorId promotorId, int page, int pageSize, CancellationToken ct);

    // Uniqueness check
    Task<bool> CodigoReferidoExistsAsync(string codigoReferido, CancellationToken ct);
}
