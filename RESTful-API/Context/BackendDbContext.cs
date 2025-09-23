using Microsoft.EntityFrameworkCore;
using RESTful.Entity;

namespace RESTful.Context;

public class BackendDbContext : DbContext
{
    public BackendDbContext(DbContextOptions<BackendDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}