using EntityFrameworkDapperApp.Core.Contracts.Services;
using EntityFrameworkDapperApp.Core.Entities.DTOs;
using EntityFrameworkDapperApp.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkDapperApp.Web.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookEntityFrameworkService _service;

    public BooksController(IBookEntityFrameworkService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IEnumerable<BookDto>> Get()
    {
        return await _service.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetById(Guid id)
    {
        var item = await _service.GetById(id);

        if (item == null)
        {
            return NotFound();
        }

        return item;
    }

    [HttpPost]
    public async Task<IActionResult> Create(BookForCreatingDto item)
    {
        var newItemId = await _service.Create(item);

        return CreatedAtAction(nameof(GetById), new { id = newItemId }, new { });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, BookForUpdatingDto item)
    {
        try
        {
            await _service.Update(id, item);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        try
        {
            await _service.Remove(id);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
}