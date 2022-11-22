using EntityFrameworkDapperApp.Core.Entities;
using EntityFrameworkDapperApp.Core.Entities.DTOs;

namespace EntityFrameworkDapperApp.Core.Contracts.Repositories;

public interface IReviewDapperRepository : IRepositoryBase<ReviewDto, Review>
{
}