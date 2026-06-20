using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Movement.API.Infrastructure.Cache;
using Movement.API.Infrastructure.CustomCache;
using Movement.API.Infrastructure.Data;
using Movement.API.Models;

namespace Movement.API.Repositories;

/// <summary>
/// Implements cache-aside (lazy population) across two cache tiers before hitting the database:
/// <list type="number">
///   <item>Redis (L1) — distributed, shared across instances</item>
///   <item>In-process LRU (L2) — fastest path, bounded to a fixed capacity</item>
///   <item>PostgreSQL — source of truth; result is written back to both caches on a miss</item>
/// </list>
/// </summary>
public class ItemRepository(AppDbContext db, IRedisService redis, CustomCacheService<string, Item> customCacheService) : IItemRepository
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public async Task<Item?> GetByIdAsync(int id)
    {
        bool isCached = false, isCustomCached = false;
        Item? item = null;
        var key = $"items:{id}";
        var cached = await redis.GetStringAsync(key);
        if (cached is not null)
        {
            isCached = true;
            item = JsonSerializer.Deserialize<Item>(cached);
        }
        else
        {
            var customCached = customCacheService.TryGet(key);
            if (customCached is not null)
            {
                isCustomCached = true;
                item = customCached;
            }
            else
            {
                item = await db.Items.FindAsync(id);
            }
        }

        if (item is not null && !isCached)
        {
            await redis.SetStringAsync(key, JsonSerializer.Serialize(item), CacheOptions);

            if (!isCustomCached)
            {
                customCacheService.AddOrUpdate(key, item);
            }
        }

        return item;
    }

    public async Task<Item> CreateAsync(Item item)
    {
        db.Items.Add(item);
        await db.SaveChangesAsync();
        return item;
    }
}
