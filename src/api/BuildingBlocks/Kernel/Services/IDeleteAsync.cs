namespace WePlayRises.BuildingBlocks.Kernel.Services
{
    public interface IDeleteAsync<TKey>
    {
        Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
    }
}
