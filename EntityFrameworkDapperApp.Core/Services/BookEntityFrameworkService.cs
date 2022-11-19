using EntityFrameworkDapperApp.Core.Contracts.Repositories;
using EntityFrameworkDapperApp.Core.Contracts.Services;
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
/*
    public async Task<BookDto?> GetById(Guid id)
    {
        return await _repository.GetById(id);
    }

    public async Task<BookAddUpdateOutputDto> Create(BookAddUpdateInputDto item)
    {
        var newItem = new Book()
        {
            Id = new Guid(),
            Title = item.Title,
            PublishedOn = item.PublishedOn,
            Image = new BookImage
            {
                Url = item.Image.Url,
                Alt = item.Image.Alt
            }
        };

        foreach (var authorId in item.Authors)
        {
            newItem.AuthorsLink.Add(new BookAuthor {BookId = newItem.Id, AuthorId = authorId});
        }

        _repository.Book.Add(newItem);
        await _repository.CommitChanges();

        var dto = new BookAddUpdateOutputDto
        {
            Id = newItem.Id,
            Title = newItem.Title,
            PublishedOn = newItem.PublishedOn,
            Image = new BookImageDto
            {
                Url = newItem.Image.Url,
                Alt = newItem.Image.Alt
            },
            Authors = newItem.AuthorsLink.Select(x => x.AuthorId)
        };

        return dto;
    }

    public async Task Update(Guid id, BookAddUpdateInputDto item)
    {
        _repository.Book.Update(id, item);
        await _repository.CommitChanges();
    }

    public async Task Remove(Guid id)
    {
        _repository.Book.Remove(id);
        await _repository.CommitChanges();
    } */
}