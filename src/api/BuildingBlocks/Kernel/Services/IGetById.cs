namespace WePlayRises.BuildingBlocks.Kernel.Services
{
    public interface IGetById<TEntity, TKey>
    {
        TEntity? GetById(TKey id);
    }
}
