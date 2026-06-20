using Microsoft.Extensions.Caching.Distributed;

namespace Movement.API.Infrastructure.Cache;

public interface IRedisService
{
    Task<string?> GetStringAsync(string key);
    Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options);
    Task RemoveAsync(string key);
}
