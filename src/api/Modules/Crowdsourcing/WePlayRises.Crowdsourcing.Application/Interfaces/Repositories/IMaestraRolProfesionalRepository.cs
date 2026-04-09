using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public interface IMaestraRolProfesionalRepository
{
    Task<MaestraRolProfesional?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<MaestraRolProfesional>> GetAllActivosAsync(CancellationToken ct);
    Task<IReadOnlyList<MaestraRolProfesional>> GetByCategoriaIdAsync(int categoriaId, CancellationToken ct);
}
