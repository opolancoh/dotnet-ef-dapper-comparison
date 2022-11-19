using EntityFrameworkDapperApp.Core.Entities.DTOs;

namespace EntityFrameworkDapperApp.Core.Contracts.Repositories;

public interface IBookDapperRepository : IRepositoryBase<BookDto, BookForCreatingDto, BookForUpdatingDto>
{
}