using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Movement.API.Infrastructure.Cache;

public class RedisService(IDistributedCache cache, IOptions<RedisSettings> options, ILogger<RedisService> logger) : IRedisService
{
    private readonly int _timeoutMs = options.Value.TimeoutMs;

    public async Task<string?> GetStringAsync(string key)
    {
        try
        {
            using var cts = new CancellationTokenSource(_timeoutMs);
            return await cache.GetStringAsync(key, cts.Token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Redis GET failed for key '{Key}'", key);
            return null;
        }
    }

    public async Task SetStringAsync(string key, string value, DistributedCacheEntryOptions opts)
    {
        try
        {
            using var cts = new CancellationTokenSource(_timeoutMs);
            await cache.SetStringAsync(key, value, opts, cts.Token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Redis SET failed for key '{Key}'", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            using var cts = new CancellationTokenSource(_timeoutMs);
            await cache.RemoveAsync(key, cts.Token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Redis REMOVE failed for key '{Key}'", key);
        }
    }
}
