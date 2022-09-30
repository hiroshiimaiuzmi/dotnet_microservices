using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Precipitation.DataAccess;

public class PrecipitationDbContext : DbContext
{
    public PrecipitationDbContext()
    {
    }
    public PrecipitationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Precipitation> Precipitation => Set<Precipitation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SnakeCaseIdentityTableNames(modelBuilder);
    }

    private static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Precipitation>(b => { b.ToTable("precipitation"); });
    }
}