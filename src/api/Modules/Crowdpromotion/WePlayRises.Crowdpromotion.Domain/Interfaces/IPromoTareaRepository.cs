using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Domain.Interfaces;

public interface IPromoTareaRepository
{
    Task<PromoTarea?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PromoTarea?> GetByIdAndProgramaAsync(Guid tareaId, PromoProgramaId programaId, CancellationToken ct);
    Task<IReadOnlyList<PromoTarea>> GetTareasByProgramaAsync(PromoProgramaId programaId, CancellationToken ct);
}
