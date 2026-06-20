using Movement.API.Models;

namespace Movement.API.Repositories;

public interface IItemRepository
{
    Task<Item?> GetByIdAsync(int id);
    Task<Item> CreateAsync(Item item);
}
