using EntityFrameworkDapperApp.Core.Contracts.Repositories;
using EntityFrameworkDapperApp.Core.Entities;
using EntityFrameworkDapperApp.Core.Entities.DTOs;
using EntityFrameworkDapperApp.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkDapperApp.Repository.EntityFramework;

public class BookEntityFrameworkRepository : IBookEntityFrameworkRepository
{
    private readonly EntityFrameworkDbContext _context;
    private readonly DbSet<Book> _entitySet;

    public BookEntityFrameworkRepository(EntityFrameworkDbContext context)
    {
        _context = context;
        _entitySet = context.Books;
    }

    public async Task<IEnumerable<BookDto>> GetAll()
    {
        var query = GetBookDtoQuery();

        return await query.ToListAsync();
    }

    public async Task<BookDto?> GetById(Guid id)
    {
        var query = GetBookDtoQuery();

        return await query.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task Create(Book item)
    {
        _entitySet.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Book item)
    {
        _context.Entry(item).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ItemExists(item.Id))
            {
                throw new EntityNotFoundException(item.Id);
            }
            else
            {
                throw;
            }
        }
    }

    public async Task Remove(Guid id)
    {
        var item = new Book { Id = id };

        _entitySet.Remove(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ItemExists(id))
            {
                throw new EntityNotFoundException(id);
            }
            else
            {
                throw;
            }
        }
    }

    public async Task<bool> ItemExists(Guid id)
    {
        return await _entitySet.AnyAsync(e => e.Id == id);
    }

    private IQueryable<BookDto> GetBookDtoQuery()
    {
        return _entitySet
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