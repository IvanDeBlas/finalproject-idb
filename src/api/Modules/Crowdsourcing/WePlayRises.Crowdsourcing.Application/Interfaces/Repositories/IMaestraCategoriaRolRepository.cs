using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public interface IMaestraCategoriaRolRepository
{
    Task<MaestraCategoriaRol?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<MaestraCategoriaRol>> GetAllAsync(CancellationToken ct);
}
