using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;

namespace WePlayRises.BuildingBlocks.Caching.Services
{
    public sealed class RequestCacheService : IRequestCacheService, IDisposable
    {
        private readonly Dictionary<string, object?> _items = new();

        public bool TryGet<T>(string key, out T? value)
        {
            if (_items.TryGetValue(key, out var obj) && obj is T t)
            {
                value = t;
                return true;
            }

            value = default;

            return false;
        }

        public async Task<T?> GetOrAddAsync<T>(string key, Func<Task<T?>> factory)
        {
            if (_items.TryGetValue(key, out var obj))
                return (T?)obj;

            var v = await factory();
            _items[key] = v;

            return v;
        }

        public void Set<T>(string key, T value) => _items[key] = value!;

        public void Dispose() => _items.Clear();
    }
}
