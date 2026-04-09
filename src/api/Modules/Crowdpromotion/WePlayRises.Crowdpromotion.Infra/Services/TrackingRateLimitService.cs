using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;

namespace WePlayRises.Crowdpromotion.Infra.Services;

public class TrackingRateLimitService : ITrackingRateLimitService
{
    private const int RateLimitWindowMinutes = 5;
    private const string CacheKeyPrefix = "ratelimit:click:";

    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<TrackingRateLimitService> _logger;

    public TrackingRateLimitService(IMemoryCache memoryCache, ILogger<TrackingRateLimitService> logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> IsRateLimitedAsync(string ipOrigen, string? codigoReferido, CancellationToken ct)
    {
        var cacheKey = $"{CacheKeyPrefix}{ipOrigen}:{codigoReferido ?? "anon"}";
        var isLimited = _memoryCache.TryGetValue(cacheKey, out _);
        return Task.FromResult(isLimited);
    }

    public Task RegisterClickAsync(string ipOrigen, string? codigoReferido, CancellationToken ct)
    {
        var cacheKey = $"{CacheKeyPrefix}{ipOrigen}:{codigoReferido ?? "anon"}";
        _memoryCache.Set(cacheKey, true, TimeSpan.FromMinutes(RateLimitWindowMinutes));
        _logger.LogDebug("Rate limit registrado para {Ip}:{CodigoReferido}", ipOrigen, codigoReferido);
        return Task.CompletedTask;
    }
}
