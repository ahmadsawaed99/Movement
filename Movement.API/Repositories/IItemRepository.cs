using Movement.API.Models;

namespace Movement.API.Repositories;

/// <summary>Data access contract for <see cref="Item"/> entities.</summary>
public interface IItemRepository
{
    /// <summary>
    /// Retrieves an item by ID using a cache-aside strategy:
    /// Redis → in-process LRU → PostgreSQL.
    /// </summary>
    Task<Item?> GetByIdAsync(int id);

    /// <summary>Persists a new item to the database.</summary>
    Task<Item> CreateAsync(Item item);
}
