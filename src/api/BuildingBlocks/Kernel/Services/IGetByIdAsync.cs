namespace WePlayRises.BuildingBlocks.Kernel.Services
{
    public interface IGetByIdAsync<TEntity, TKey>
    {
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    }
}
