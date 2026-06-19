using Microsoft.EntityFrameworkCore;
using Movement.API.Models;

namespace Movement.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();
}
