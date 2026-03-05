using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Application.Interfaces.Services;

public interface IArtistaService
{
    Task<Artista?> GetByIdAsync(ArtistaId id, CancellationToken ct);
    Task<Artista?> GetByUserIdAsync(string userId, CancellationToken ct);
    Task<bool> ExistsForUserAsync(string userId, CancellationToken ct);
    Task<ArtistaId> CreateAsync(Artista entity, CancellationToken ct);
    Task UpdateAsync(Artista entity, CancellationToken ct);
}
