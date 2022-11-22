namespace EntityFrameworkDapperApp.Core.Contracts.Services;

public interface IPersistenceServiceBase<TDto, in TCreate, TUpdate>
{
    Task<IEnumerable<TDto>> GetAll();
    Task<TDto?> GetById(Guid id);
    Task<Guid> Create(TCreate item);
    Task Update(Guid id, TUpdate item);
    Task Remove(Guid id);
}