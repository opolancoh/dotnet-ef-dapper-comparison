using EntityFrameworkDapperApp.Core.Entities.DTOs;

namespace EntityFrameworkDapperApp.Core.Contracts.Repositories;

public interface IBookEntityFrameworkRepository : IRepositoryBase<BookDto, BookForCreatingDto, BookForUpdatingDto>
{
}