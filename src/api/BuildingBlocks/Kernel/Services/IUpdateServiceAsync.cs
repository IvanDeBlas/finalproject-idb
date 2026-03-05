namespace WePlayRises.BuildingBlocks.Kernel.Services
{
    public interface IUpdateServiceAsync<TEntity, TKey> : IUpdateAsync<TEntity>, IItemExistsAsync<TKey>
        where TKey : IEquatable<TKey>
    { }
}
