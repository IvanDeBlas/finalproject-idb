using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Interfaces.Services;

public interface IPromoTareaService
{
    // Identity resolution
    Task<Promotor?> GetPromotorByUserIdAsync(string userId, CancellationToken ct);
    Task<ArtistaId?> GetArtistaIdByUserIdAsync(string userId, CancellationToken ct);

    // PromoPrograma access
    Task<PromoPrograma?> GetProgramaByIdAsync(PromoProgramaId programaId, CancellationToken ct);

    // Inscripcion access
    Task<PromoProgramaPromotor?> GetInscripcionAprobadaAsync(PromotorId promotorId, PromoProgramaId programaId, CancellationToken ct);

    // PromoTarea access
    Task<PromoTarea?> GetTareaByIdAndProgramaAsync(Guid tareaId, PromoProgramaId programaId, CancellationToken ct);

    // PromoTareaPromotor access
    Task<PromoTareaPromotor?> GetTareaPromotorByIdAsync(Guid tareaPromotorId, CancellationToken ct);
    Task<int> ContarCompletadosActivosAsync(Guid tareaId, Guid programaPromotorId, CancellationToken ct);
    Task<PromoTareaPromotor?> GetUltimoCompletadoAsync(Guid tareaId, Guid programaPromotorId, CancellationToken ct);

    // Orchestration methods (return DTOs)
    Task<MisTareasResponseDto> GetTareasActivasConEstadoAsync(PromoProgramaId programaId, Guid programaPromotorId, CancellationToken ct);
    Task<(CompletarTareaResponseDto Dto, int VecesCompletada)> CompletarTareaAsync(PromoTareaPromotor registro, bool esActualizacion, CancellationToken ct);
    Task<(IReadOnlyList<TareaPendienteItemDto> Items, int TotalCount)> GetTareasPendientesAsync(PromoProgramaId programaId, int page, int pageSize, CancellationToken ct);
    Task<ValidarTareaResponseDto> ValidarTareaAsync(PromoTareaPromotor registro, PromoProgramaId programaId, string? comentario, CancellationToken ct);
    Task RechazarTareaAsync(PromoTareaPromotor registro, string comentario, CancellationToken ct);
}
