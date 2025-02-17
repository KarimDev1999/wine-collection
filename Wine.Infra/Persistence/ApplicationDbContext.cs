using Microsoft.EntityFrameworkCore;

namespace Wine.Infra.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Domain.Entities.Wine> Wines { get; set; }
}
