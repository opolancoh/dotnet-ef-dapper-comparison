using EntityFrameworkDapperApp.Core.Entities;
using EntityFrameworkDapperApp.Core.Entities.DTOs;

namespace EntityFrameworkDapperApp.Core.Contracts.Repositories;

public interface IBookEntityFrameworkRepository : IRepositoryBase<BookDto, Book>
{
}