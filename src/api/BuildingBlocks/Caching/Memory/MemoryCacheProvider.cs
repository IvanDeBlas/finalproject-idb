using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using WePlayRises.BuildingBlocks.Caching.Memory.Interfaces;
using WePlayRises.BuildingBlocks.Caching.Model;
using WePlayRises.BuildingBlocks.Kernel.Model;
using System.Diagnostics.CodeAnalysis;

namespace WePlayRises.BuildingBlocks.Caching.Memory
{
    [ExcludeFromCodeCoverage]
    public class MemoryCacheProvider : IMemoryCacheProvider
    {
        private readonly HashSet<string> _cacheKeys = new HashSet<string>();

        private static readonly object _lock = new object();

        public CacheOptions Config { get; set; }
        public MemoryCacheEntryOptions CacheItemPolicy { get; set; }
        public MemoryCache Cache { get; set; }


        public MemoryCacheProvider(IMemoryCache cache, IOptions<CacheOptions> config,
            IOptions<MemoryCacheEntryOptions> options)
        {
            Cache = (MemoryCache)(cache ?? throw new ArgumentNullException(nameof(cache)));
            Config = config.Value ?? throw new ArgumentNullException(nameof(config));
            CacheItemPolicy = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public TEntity? Get<TEntity>(string key)
        {
            return Cache.Get<TEntity>(key);
        }

        public IEnumerable<TEntity>? GetCollection<TEntity>(string key)
        {
            var entity = Cache.Get(key);
            if (entity == null)
                return null;

            var entityCollection = (EntityCollection<TEntity, string>)entity;
            return entityCollection?.Collection;
        }

        public TEntity Set<TEntity>(TEntity entity, string key)
        {
            lock (_lock)
            {
                _cacheKeys.Add(key);
                var result = Cache.Set(key, entity, CacheItemPolicy);
                if (Cache?.Count > Config.MaxCacheSize)
                    Cache?.Compact((double)(Cache.Count - Config.MaxCacheSize) / Cache.Count);

                return result;
            }
        }

        public TEntity Set<TEntity>(TEntity entity, string key, TimeSpan timeSpan)
        {
            return Cache.Set(key, entity, timeSpan);
        }

        public EntityCollection<TEntity, string> SetCollection<TEntity>(IEnumerable<TEntity> collection, string key)
        {
            return Cache.Set(key, new EntityCollection<TEntity, string>
            {
                Id = key,
                Collection = collection
            }, CacheItemPolicy);
        }

        public EntityCollection<TEntity, string> SetCollection<TEntity>(IEnumerable<TEntity> collection, string key, TimeSpan timeSpan)
        {
            return Cache.Set(key, new EntityCollection<TEntity, string>
            {
                Id = key,
                Collection = collection
            }, timeSpan);
        }

        public void Remove(string key) => Cache.Remove(key);


        public void ClearCache()
        {
            foreach (var key in _cacheKeys)
            {
                Cache.Remove(key);
            }
            _cacheKeys.Clear();
        }
    }
}
