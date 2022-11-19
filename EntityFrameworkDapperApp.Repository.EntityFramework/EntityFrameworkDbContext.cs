using EntityFrameworkDapperApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkDapperApp.Repository.EntityFramework;

public class EntityFrameworkDbContext : DbContext
{
    public EntityFrameworkDbContext(DbContextOptions<EntityFrameworkDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Review> Reviews { get; set; }
}