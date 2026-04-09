using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Application.Interfaces.Services;

public interface IPerfilProfesionalService
{
    Task<PerfilProfesional?> GetByUserIdAsync(string userId, CancellationToken ct);
    Task<bool> ExistsByUserIdAsync(string userId, CancellationToken ct);
}
