using Microsoft.Extensions.Caching.Memory;
using WePlayRises.BuildingBlocks.Caching.Model;
using WePlayRises.BuildingBlocks.Kernel.Model;

namespace WePlayRises.BuildingBlocks.Caching.Memory.Interfaces
{
    public interface IMemoryCacheProvider
    {
        CacheOptions Config { get; set; }
        MemoryCacheEntryOptions CacheItemPolicy { get; set; }
        MemoryCache Cache { get; set; }

        TEntity? Get<TEntity>(string key);

        IEnumerable<TEntity>? GetCollection<TEntity>(string key);

        TEntity Set<TEntity>(TEntity entity, string key);

        TEntity Set<TEntity>(TEntity entity, string key, TimeSpan timeSpan);

        EntityCollection<TEntity, string> SetCollection<TEntity>(IEnumerable<TEntity> collection, string key);

        EntityCollection<TEntity, string> SetCollection<TEntity>(IEnumerable<TEntity> collection, string key, TimeSpan timeSpan);

        void Remove(string key);
        void ClearCache();
    }
}
