namespace WePlayRises.BuildingBlocks.Caching.Services.Interfaces
{
    public interface IRequestCacheService
    {
        bool TryGet<T>(string key, out T? value);
        Task<T?> GetOrAddAsync<T>(string key, Func<Task<T?>> factory);
        void Set<T>(string key, T value);
    }
}
