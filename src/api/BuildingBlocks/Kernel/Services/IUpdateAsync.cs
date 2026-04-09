namespace WePlayRises.BuildingBlocks.Kernel.Services
{
    public interface IUpdateAsync<TEntity>
    {
        Task<bool> UpdateAsync(TEntity item, CancellationToken cancellationToken = default);
    }
}
