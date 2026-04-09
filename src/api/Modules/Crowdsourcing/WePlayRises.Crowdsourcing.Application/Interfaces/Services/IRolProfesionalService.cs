using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Services;

public interface IRolProfesionalService
{
    Task<MaestraRolProfesional?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<MaestraRolProfesional>> GetAllActivosAsync(CancellationToken ct);
    Task<IReadOnlyList<MaestraRolProfesional>> GetByCategoriaIdAsync(int categoriaId, CancellationToken ct);
}
