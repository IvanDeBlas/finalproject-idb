namespace WePlayRises.BuildingBlocks.Kernel.Services
{
    public interface IItemExistsAsync<TKey>
    {
        Task<bool> ItemExistsAsync(TKey id, CancellationToken cancellationToken = default);
        Task<(bool exists, TKey? id)> ItemExistsWithNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
