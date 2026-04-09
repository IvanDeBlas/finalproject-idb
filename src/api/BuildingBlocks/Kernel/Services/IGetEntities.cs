using WePlayRises.BuildingBlocks.Kernel.Model;

namespace WePlayRises.BuildingBlocks.Kernel.Services
{
    public interface IGetEntities<TEntity>
        where TEntity : class
    {
        PagedList<TEntity> GetEntities(SearchFilter filter);
    }
}
