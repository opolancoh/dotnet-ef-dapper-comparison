namespace EntityFrameworkDapperApp.Core.Contracts.Repositories;

public interface IRepositoryBase<TDto, in TEntity>
{
    Task<IEnumerable<TDto>> GetAll();
    Task<TDto?> GetById(Guid id);
    Task Create(TEntity item);
    Task Update(TEntity item);
    Task Remove(Guid id);
    Task<bool> ItemExists(Guid id);
}