using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Services;

public interface ICategoriaRolService
{
    Task<MaestraCategoriaRol?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<MaestraCategoriaRol>> GetAllAsync(CancellationToken ct);
}
