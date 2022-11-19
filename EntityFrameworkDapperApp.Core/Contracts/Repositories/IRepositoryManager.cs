namespace EntityFrameworkDapperApp.Core.Contracts.Repositories;

public interface IRepositoryManager
{
    IBookRepository Book { get; }
    IReviewRepository Review { get; }
    Task CommitChanges();
}