using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public interface IPlantillaProyectoRepository
{
    Task<PlantillaProyecto?> GetByIdAsync(PlantillaProyectoId id, CancellationToken ct);
    Task<PlantillaProyecto?> GetByIdWithNecesidadesAsync(PlantillaProyectoId id, CancellationToken ct);
    Task<IReadOnlyList<PlantillaProyecto>> GetAllActivosAsync(CancellationToken ct);
    Task<PlantillaProyectoId> AddAsync(PlantillaProyecto entity, CancellationToken ct);
    Task UpdateAsync(PlantillaProyecto entity, CancellationToken ct);
    Task DeleteAsync(PlantillaProyectoId id, CancellationToken ct);
}
