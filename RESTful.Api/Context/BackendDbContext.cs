using Microsoft.EntityFrameworkCore;
using RESTful.Api.Entity;

namespace RESTful.Api.Context;

public class BackendDbContext : DbContext
{
    public BackendDbContext(DbContextOptions<BackendDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}