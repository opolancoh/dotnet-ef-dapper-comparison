using EntityFrameworkDapperApp.Core.Contracts.Repositories;
using EntityFrameworkDapperApp.Core.Contracts.Services;
using EntityFrameworkDapperApp.Core.Entities;
using EntityFrameworkDapperApp.Core.Entities.DTOs;

namespace EntityFrameworkDapperApp.Core.Services;

public sealed class BookEntityFrameworkService : IBookEntityFrameworkService
{
    private readonly IBookEntityFrameworkRepository _repository;

    public BookEntityFrameworkService(IBookEntityFrameworkRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BookDto>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<BookDto?> GetById(Guid id)
    {
        return await _repository.GetById(id);
    }

    public async Task<Guid> Create(BookForCreatingDto item)
    {
        var newItem = new Book()
        {
            Id = Guid.NewGuid(),
            Title = item.Title!,
            PublishedOn = item.PublishedOn!.Value
        };

        await _repository.Create(newItem);

        return newItem.Id;
    }

    public async Task Update(Guid id, BookForUpdatingDto item)
    {
        var itemToUpdate = new Book()
        {
            Id = id,
            Title = item.Title!,
            PublishedOn = item.PublishedOn!.Value
        };

        await _repository.Update(itemToUpdate);
    }

    public async Task Remove(Guid id)
    {
        await _repository.Remove(id);
    }
}