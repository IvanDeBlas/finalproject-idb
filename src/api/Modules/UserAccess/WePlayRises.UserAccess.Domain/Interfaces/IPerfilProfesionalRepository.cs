using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Domain.Interfaces;

public interface IPerfilProfesionalRepository
{
    Task<PerfilProfesional?> GetByUserIdAsync(string userId, CancellationToken ct);
    Task<bool> ExistsByUserIdAsync(string userId, CancellationToken ct);
}
