using Microsoft.AspNetCore.Mvc;
using Movement.API.Models;
using Movement.API.Repositories;

namespace Movement.API.Controllers;

/// <summary>Manages item resources.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ItemsController(IItemRepository repository) : ControllerBase
{
    /// <summary>Gets a single item by its ID.</summary>
    /// <param name="id">The unique identifier of the item.</param>
    /// <response code="200">Item found and returned.</response>
    /// <response code="404">No item exists with the given ID.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Item>> GetById(int id)
    {
        var item = await repository.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>Creates a new item.</summary>
    /// <param name="item">The item data to persist.</param>
    /// <response code="201">Item created successfully; Location header points to the new resource.</response>
    /// <response code="400">Request body is invalid or missing required fields.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Item), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Item>> Create(Item item)
    {
        var created = await repository.CreateAsync(item);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
