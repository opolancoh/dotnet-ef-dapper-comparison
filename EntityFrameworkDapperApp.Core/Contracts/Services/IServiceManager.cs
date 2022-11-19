namespace EntityFrameworkDapperApp.Core.Contracts.Services;

public interface IServiceManager
{
    IBookService BookService { get; }
    IReviewService ReviewService { get; }
}