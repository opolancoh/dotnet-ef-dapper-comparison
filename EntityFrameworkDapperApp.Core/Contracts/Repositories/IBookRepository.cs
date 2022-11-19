using EntityFrameworkDapperApp.Core.Entities.DTOs;

namespace EntityFrameworkDapperApp.Core.Contracts.Repositories;

public interface IBookRepository : IRepositoryBase<BookDto, BookForCreatingDto, BookForUpdatingDto>
{
}