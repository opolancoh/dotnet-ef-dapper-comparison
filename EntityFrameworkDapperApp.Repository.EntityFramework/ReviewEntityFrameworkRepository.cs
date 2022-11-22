using EntityFrameworkDapperApp.Core.Contracts.Repositories;
using EntityFrameworkDapperApp.Core.Entities;
using EntityFrameworkDapperApp.Core.Entities.DTOs;
using EntityFrameworkDapperApp.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkDapperApp.Repository.EntityFramework;

public class ReviewEntityFrameworkRepository : IReviewEntityFrameworkRepository
{
    private readonly EntityFrameworkDbContext _context;
    private readonly DbSet<Review> _entitySet;

    public ReviewEntityFrameworkRepository(EntityFrameworkDbContext context)
    {
        _context = context;
        _entitySet = context.Reviews;
    }

    public async Task<IEnumerable<ReviewDto>> GetAll()
    {
        var query = GetReviewDtoQuery();

        return await query.ToListAsync();
    }

    public async Task<ReviewDto?> GetById(Guid id)
    {
        var query = GetReviewDtoQuery();

        return await query.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task Create(Review item)
    {
        _entitySet.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Review item)
    {
        _context.Entry(item).State = EntityState.Modified;
        _context.Entry(item).Property(x => x.BookId).IsModified = false;

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
        var item = new Review { Id = id };

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

    private IQueryable<ReviewDto> GetReviewDtoQuery()
    {
        return _entitySet
            .AsNoTracking()
            .Select(x => new ReviewDto
            {
                Id = x.Id,
                Comment = x.Comment,
                Rating = x.Rating,
                BookId = x.BookId
            });
    }
}