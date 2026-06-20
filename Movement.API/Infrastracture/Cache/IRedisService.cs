using Microsoft.Extensions.Caching.Distributed;

namespace Movement.API.Infrastructure.Cache;

/// <summary>
/// Fault-tolerant Redis abstraction. All operations are non-throwing:
/// on connectivity failure they log a warning and return a safe default,
/// allowing callers to fall back to the next cache tier or the database.
/// </summary>
public interface IRedisService
{
    /// <summary>
    /// Returns the cached string for <paramref name="key"/>,
    /// or <c>null</c> if the key is absent or Redis is unavailable.
    /// </summary>
    Task<string?> GetStringAsync(string key);

    /// <summary>
    /// Stores <paramref name="value"/> under <paramref name="key"/> with the given expiry.
    /// Silently no-ops if Redis is unavailable.
    /// </summary>
    Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options);

    /// <summary>
    /// Removes the entry for <paramref name="key"/>.
    /// Silently no-ops if Redis is unavailable.
    /// </summary>
    Task RemoveAsync(string key);
}
