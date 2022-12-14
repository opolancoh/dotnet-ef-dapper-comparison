using EntityFrameworkDapperApp.Core.Contracts.Services;
using EntityFrameworkDapperApp.Core.Entities.DTOs;
using EntityFrameworkDapperApp.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkDapperApp.Web.Controllers.v2;

[ApiController]
[ApiVersion("2.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewDapperService _service;

    public ReviewsController(IReviewDapperService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IEnumerable<ReviewDto>> Get()
    {
        return await _service.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetById(Guid id)
    {
        var item = await _service.GetById(id);

        if (item == null)
        {
            return NotFound();
        }

        return item;
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReviewCreateDto item)
    {
        var newItemId = await _service.Create(item);

        return CreatedAtAction(nameof(GetById), new { id = newItemId }, new { });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ReviewUpdateDto item)
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