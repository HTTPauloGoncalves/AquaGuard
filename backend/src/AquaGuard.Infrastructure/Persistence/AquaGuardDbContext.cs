using AquaGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquaGuard.Infrastructure.Persistence;

public sealed class AquaGuardDbContext(DbContextOptions<AquaGuardDbContext> options) : DbContext(options)
{
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Reservoir> Reservoirs => Set<Reservoir>();
    public DbSet<Measurement> Measurements => Set<Measurement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AquaGuardDbContext).Assembly);
    }
}
