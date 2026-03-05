using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public interface IPlantillaProyectoNecesidadRepository
{
    Task<PlantillaProyectoNecesidad?> GetByIdAsync(PlantillaProyectoNecesidadId id, CancellationToken ct);
    Task<IReadOnlyList<PlantillaProyectoNecesidad>> GetByPlantillaIdAsync(PlantillaProyectoId plantillaId, CancellationToken ct);
    Task<IReadOnlyList<PlantillaProyectoNecesidad>> GetByIdsAsync(IEnumerable<PlantillaProyectoNecesidadId> ids, CancellationToken ct);
    Task<PlantillaProyectoNecesidadId> AddAsync(PlantillaProyectoNecesidad entity, CancellationToken ct);
    Task UpdateAsync(PlantillaProyectoNecesidad entity, CancellationToken ct);
    Task DeleteAsync(PlantillaProyectoNecesidadId id, CancellationToken ct);
}
