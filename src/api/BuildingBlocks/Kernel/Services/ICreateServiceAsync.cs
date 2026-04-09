namespace WePlayRises.BuildingBlocks.Kernel.Services
{
    public interface ICreateServiceAsync<TEntity, TKey> : ICreateAsync<TEntity, TKey>, IItemExistsAsync<TKey>
        where TKey : IEquatable<TKey>
    { }
}
