using EntityFrameworkDapperApp.Repository.Dapper;
using EntityFrameworkDapperApp.Repository.EntityFramework;
using EntityFrameworkDapperApp.Tests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkDapperApp.Tests.IntegrationTests.Fixtures;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var connectionString =
                "Server=localhost; Database=books_ef_dapper_db_test; Username=postgres; Password=My@Passw0rd;";
            // Entity Framework DbContext
            // Remove current
            var entityFrameworkDbContext = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<EntityFrameworkDbContext>));
            if (entityFrameworkDbContext != null) services.Remove(entityFrameworkDbContext);
            // Add a new one
            services.AddDbContext<EntityFrameworkDbContext>(options => options
                .UseNpgsql(connectionString));
            
            // Dapper DbContext
            // Remove current
            var dapperDbContext = services.SingleOrDefault(x => x.ServiceType == typeof(DapperDbContext));
            if (dapperDbContext != null) services.Remove(dapperDbContext);
            // Add a new one
            services.AddSingleton(x => new DapperDbContext(connectionString));
            
            //
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<EntityFrameworkDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // Don't update/remove this initial data
            var books = DbHelper.Books;
            db.Books?.AddRange(books);
            db.SaveChanges();

            var reviews = DbHelper.Reviews;
            db.Reviews?.AddRange(reviews);
            db.SaveChanges();

            logger.LogError("All data was saved successfully"); 
        });
    }
}