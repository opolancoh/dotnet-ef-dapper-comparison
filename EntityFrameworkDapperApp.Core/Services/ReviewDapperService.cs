using EntityFrameworkDapperApp.Core.Contracts.Repositories;
using EntityFrameworkDapperApp.Core.Contracts.Services;
using EntityFrameworkDapperApp.Core.Entities;
using EntityFrameworkDapperApp.Core.Entities.DTOs;

namespace EntityFrameworkDapperApp.Core.Services;

public sealed class ReviewDapperService : IReviewDapperService
{
    private readonly IReviewDapperRepository _repository;

    public ReviewDapperService(IReviewDapperRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReviewDto>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<ReviewDto?> GetById(Guid id)
    {
        return await _repository.GetById(id);
    }

    public async Task<Guid> Create(ReviewCreateDto item)
    {
        var newItem = new Review
        {
            Id = Guid.NewGuid(),
            Comment = item.Comment!,
            Rating = item.Rating!.Value,
            BookId = item.BookId!.Value
        };

        await _repository.Create(newItem);

        return newItem.Id;
    }

    public async Task Update(Guid id, ReviewUpdateDto item)
    {
        var itemToUpdate = new Review
        {
            Id = id,
            Comment = item.Comment!,
            Rating = item.Rating!.Value
        };

        await _repository.Update(itemToUpdate);
    }

    public async Task Remove(Guid id)
    {
        await _repository.Remove(id);
    }
}