namespace WePlayRises.BuildingBlocks.Kernel.Services
{
    public interface ICreateAsync<TEntity, TKey>
        where TKey : IEquatable<TKey>
    {
        Task<TKey> CreateAsync(TEntity item, CancellationToken cancellationToken = default);
    }
}
