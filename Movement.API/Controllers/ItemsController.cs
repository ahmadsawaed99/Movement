using Microsoft.AspNetCore.Mvc;
using Movement.API.Models;
using Movement.API.Repositories;

namespace Movement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController(IItemRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetAll() =>
        Ok(await repository.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Item>> GetById(int id)
    {
        var item = await repository.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Item>> Create(Item item)
    {
        var created = await repository.CreateAsync(item);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
