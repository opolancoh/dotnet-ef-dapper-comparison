using EntityFrameworkDapperApp.Core.Contracts.Services;
using EntityFrameworkDapperApp.Core.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkDapperApp.Web.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
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
/*
    [HttpGet("{id}")]
    public async Task<ActionResult<BookDetailDto>> GetById(Guid id)
    {
        var item = await _service.BookService.GetById(id);

        if (item == null)
        {
            return NotFound();
        }

        return item;
    }
    
    [HttpPost]
    public async Task<ActionResult<BookAddUpdateOutputDto>> Add(BookAddUpdateInputDto item)
    {
        var newItem = await _service.BookService.Add(item);

        return CreatedAtAction(nameof(GetById), new {id = newItem.Id}, newItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, BookAddUpdateInputDto item)
    {
        try
        {
            await _service.BookService.Update(id, item);
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
            await _service.BookService.Remove(id);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
    */
}