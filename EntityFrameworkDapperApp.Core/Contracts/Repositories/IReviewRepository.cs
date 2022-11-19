using EntityFrameworkDapperApp.Core.Entities.DTOs;

namespace EntityFrameworkDapperApp.Core.Contracts.Repositories;

public interface IReviewRepository : IRepositoryBase<ReviewDto, ReviewForCreatingDto, ReviewForUpdatingDto>
{
}