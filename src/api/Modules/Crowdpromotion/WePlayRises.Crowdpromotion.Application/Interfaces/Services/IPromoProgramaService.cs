using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Interfaces.Services;

public interface IPromoProgramaService
{
    Task<ArtistaId?> GetArtistaIdByUserIdAsync(string userId, CancellationToken ct);
    Task<PromoPrograma?> GetByIdAsync(PromoProgramaId id, CancellationToken ct);
    Task<PromoPrograma?> GetByIdWithTareasAsync(PromoProgramaId id, CancellationToken ct);
    Task<PromoPrograma?> GetByIdWithFullDetailAsync(PromoProgramaId id, CancellationToken ct);
    Task<(IReadOnlyList<PromoPrograma> Items, int TotalCount)> GetMisProgramasAsync(
        ArtistaId artistaId, bool? esActivo, int page, int pageSize, CancellationToken ct);
    Task<bool> ExisteCodigoTrackingAsync(string codigoTracking, ArtistaId artistaId, CancellationToken ct);
    Task<bool> ExisteCodigoTrackingParaEdicionAsync(
        string codigoTracking, ArtistaId artistaId, PromoProgramaId excludeId, CancellationToken ct);
    Task<bool> CodigoTrackingExistsAsync(string? userId, string? codigo, Guid? excludeId, CancellationToken ct);
    Task<PromoProgramaId> CreateWithTareasAsync(PromoPrograma programa, IReadOnlyList<PromoTarea> tareas, CancellationToken ct);
    Task UpdateWithTareasAsync(
        PromoPrograma programa,
        IReadOnlyList<PromoTarea> tareasNuevas,
        IReadOnlyList<PromoTarea> tareasActualizar,
        IReadOnlyList<Guid> tareasDesactivar,
        CancellationToken ct);
    Task<int> DesactivarWithTareasAsync(PromoProgramaId id, CancellationToken ct);
    Task<bool> TipoPromoExistsAsync(int tipoPromoId, CancellationToken ct);
    Task<bool> TipoEventoPromoExistsAsync(int tipoEventoPromoId, CancellationToken ct);
    Task<bool> TipoRewardExistsAsync(int tipoRewardId, CancellationToken ct);
    Task<bool> MonedaExistsAsync(int monedaId, CancellationToken ct);
    Task<bool> VerificarCampaniaPertenece(ArtistaId artistaId, Guid campaniaCrowdfundingId, CancellationToken ct);
    Task<bool> VerificarProyectoPertenece(ArtistaId artistaId, Guid proyectoArtisticoId, CancellationToken ct);
    Task<bool> TareaConCompletadosAsync(Guid tareaId, CancellationToken ct);
    Task<PromoProgramaResumenDto> GetResumenAsync(PromoProgramaId programaId, CancellationToken ct);
}
