using EntityFrameworkDapperApp.Core.Contracts.Repositories;
using EntityFrameworkDapperApp.Core.Entities.DTOs;

namespace EntityFrameworkDapperApp.Core.Contracts.Services;

public interface IReviewEntityFrameworkService : IServiceBase<ReviewDto, ReviewCreateDto, ReviewUpdateDto>
{
}