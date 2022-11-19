using Microsoft.EntityFrameworkCore;
using EntityFrameworkDapperApp.Core.Contracts.Repositories;
using EntityFrameworkDapperApp.Core.Contracts.Services;
using EntityFrameworkDapperApp.Core.Services;
using EntityFrameworkDapperApp.Repository.EntityFramework;

namespace EntityFrameworkDapperApp.Web.Extensions;

public static class ServiceExtensions
{
    public static void ConfigurePersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookService, BookService>();
    }

    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<EntityFrameworkDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("DbConnection")));
}