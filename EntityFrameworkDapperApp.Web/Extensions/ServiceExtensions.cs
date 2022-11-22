using Microsoft.EntityFrameworkCore;
using EntityFrameworkDapperApp.Core.Contracts.Repositories;
using EntityFrameworkDapperApp.Core.Contracts.Services;
using EntityFrameworkDapperApp.Core.Services;
using EntityFrameworkDapperApp.Repository.Dapper;
using EntityFrameworkDapperApp.Repository.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace EntityFrameworkDapperApp.Web.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });
    }

    public static void ConfigurePersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<IBookEntityFrameworkRepository, BookEntityFrameworkRepository>();
        services.AddScoped<IBookDapperRepository, BookDapperRepository>();
        services.AddScoped<IReviewEntityFrameworkRepository, ReviewEntityFrameworkRepository>();
        services.AddScoped<IReviewDapperRepository, ReviewDapperRepository>();

        services.AddScoped<IBookEntityFrameworkService, BookEntityFrameworkService>();
        services.AddScoped<IBookDapperService, BookDapperService>();
        services.AddScoped<IReviewEntityFrameworkService, ReviewEntityFrameworkService>();
        services.AddScoped<IReviewDapperService, ReviewDapperService>();
    }

    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConnection = configuration.GetConnectionString("DbConnection");

        services.AddDbContext<EntityFrameworkDbContext>(opts => opts.UseNpgsql(dbConnection));

        services.AddSingleton(x => new DapperDbContext(dbConnection));
    }
}