using WePlayRises.BuildingBlocks.Kernel.Model;

namespace WePlayRises.BuildingBlocks.Kernel.Services
{
    public interface IGetEntitiesAsync<TEntity>
        where TEntity : class
    {
        Task<PagedList<TEntity>> GetEntitiesAsync(SearchFilter filter, CancellationToken cancellationToken = default);
    }
}
