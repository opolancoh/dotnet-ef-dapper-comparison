namespace EntityFrameworkDapperApp.Core.Contracts.Services;

public interface IPersistenceServiceBase<TModel, TCreate, TUpdate>
{
    Task<IEnumerable<TModel>> GetAll();
    /* Task<TModel?> GetById(Guid id);
    Task<Guid> Create(TCreate item);
    Task Update(TUpdate item);
    Task Remove(Guid id);
    Task<bool> ItemExists(Guid id); */
}