using Microsoft.EntityFrameworkCore;
using RESTful.Entity;

namespace RESTful.Context;

public class BackendDBContext : DbContext
{
    public BackendDBContext(DbContextOptions<BackendDBContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}