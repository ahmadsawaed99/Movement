using Microsoft.EntityFrameworkCore;
using Movement.API.Data;
using Movement.API.Models;

namespace Movement.API.Repositories;

public class ItemRepository(AppDbContext db) : IItemRepository
{
    public async Task<IEnumerable<Item>> GetAllAsync() =>
        await db.Items.ToListAsync();

    public async Task<Item?> GetByIdAsync(int id) =>
        await db.Items.FindAsync(id);

    public async Task<Item> CreateAsync(Item item)
    {
        db.Items.Add(item);
        await db.SaveChangesAsync();
        return item;
    }
}
