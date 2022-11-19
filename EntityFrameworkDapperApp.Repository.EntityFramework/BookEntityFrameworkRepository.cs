using EntityFrameworkDapperApp.Core.Contracts.Repositories;
using EntityFrameworkDapperApp.Core.Entities;
using EntityFrameworkDapperApp.Core.Entities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkDapperApp.Repository.EntityFramework;

public class BookEntityFrameworkRepository : IBookEntityFrameworkRepository
{
    private readonly DbSet<Book> _entityItems;

    public BookEntityFrameworkRepository(EntityFrameworkDbContext context)
    {
        _entityItems = context.Books;
    }

    public async Task<IEnumerable<BookDto>> GetAll()
    {
        var query = GetBookDtoQuery();

        return await query.ToListAsync();
    }

/*
    public async Task<BookDetailDto?> GetById(Guid id)
    {
        var query = GetBookDetailDtoQuery();

        return await query.SingleOrDefaultAsync(x => x.Id == id);
    }

    public void Add(Book item)
    {
        AddOne(item);
    }

    public void Update(Guid id, BookAddUpdateInputDto item)
    {
        var currentItem = _entityItems
            .Include(x => x.Image)
            .Include(x => x.AuthorsLink)
            .FirstOrDefault(x => x.Id == id);

        if (currentItem == null)
        {
            throw new EntityNotFoundException(id);
        }

        // Main update
        currentItem.Title = item.Title;
        currentItem.PublishedOn = item.PublishedOn;

        // Image update
        currentItem.Image.Url = item.Image.Url;
        currentItem.Image.Alt = item.Image.Alt;

        // Authors update
        var authorsToAdd = item.Authors
            .Where(x => currentItem.AuthorsLink.All(y => y.AuthorId != x))
            .Select(x => new BookAuthor {BookId = id, AuthorId = x});
        foreach (var authorToAdd in authorsToAdd)
        {
            currentItem.AuthorsLink.Add(authorToAdd);
        }

        var authorsToRemove = currentItem.AuthorsLink
            .Where(x => item.Authors.All(y => y != x.AuthorId))
            .ToList();
        foreach (var authorToRemove in authorsToRemove)
        {
            currentItem.AuthorsLink.Remove(authorToRemove);
        }

        UpdateOne(currentItem);
    }

    public void Remove(Guid id)
    {
        var currentItem = _entityItems
            .Include(x => x.Image)
            .Include(x => x.AuthorsLink)
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);

        if (currentItem == null)
        {
            throw new EntityNotFoundException(id);
        }

        RemoveOne(currentItem);
    }
*/
    private IQueryable<BookDto> GetBookDtoQuery()
    {
        return _entityItems
            .AsNoTracking()
            .Select(x => new BookDto
            {
                Id = x.Id,
                Title = x.Title,
                PublishedOn = x.PublishedOn,
                Reviews = x.Reviews.Select(y => new ReviewBaseDto
                {
                    Id = y.Id,
                    Comment = y.Comment,
                    Rating = y.Rating
                })
            });
    }
}