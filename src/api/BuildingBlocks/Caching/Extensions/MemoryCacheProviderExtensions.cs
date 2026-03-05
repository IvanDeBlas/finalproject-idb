using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WePlayRises.BuildingBlocks.Caching.Memory;
using WePlayRises.BuildingBlocks.Caching.Memory.Interfaces;
using WePlayRises.BuildingBlocks.Caching.Model;
using WePlayRises.BuildingBlocks.Caching.Services;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;

namespace WePlayRises.BuildingBlocks.Caching.Extensions
{
    public static class MemoryCacheProviderExtensions
    {
        public static IServiceCollection AddMemoryCacheProvider(this IServiceCollection services, IConfiguration configuration)
        {
            // ToDo: Crear el Options Patron: https://chatgpt.com/share/67f6986d-2f60-8010-a263-1c482ef17943

            services.Configure<CacheOptions>(configuration.GetSection("CacheOptions"));
            services.Configure<MemoryCacheEntryOptions>(configuration.GetSection("CacheOptions").GetSection("MemoryCacheEntryOptions"));
            services.AddMemoryCache();
            services.AddSingleton<IMemoryCacheProvider, MemoryCacheProvider>();

            return services;
        }


        public static IServiceCollection AddRequestCacheService(this IServiceCollection services)
        {
            // ToDo: https://chatgpt.com/c/68dfd635-70c8-8330-b9b3-9b9280493944

            services.AddScoped<IRequestCacheService, RequestCacheService>();

            return services;
        }
    }
}