using AquaGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquaGuard.Infrastructure.Persistence;

public static class DemoDataSeeder
{
    public static readonly Guid PropertyId = Guid.Parse("00000000-0000-0000-0000-000000000010");
    public static readonly Guid ReservoirId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static async Task SeedAsync(AquaGuardDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Properties.AnyAsync(p => p.Id == PropertyId, cancellationToken))
        {
            dbContext.Properties.Add(new Property(PropertyId, "Propriedade de demonstração"));
        }

        if (!await dbContext.Reservoirs.AnyAsync(r => r.Id == ReservoirId, cancellationToken))
        {
            dbContext.Reservoirs.Add(new Reservoir(ReservoirId, PropertyId,
                "Açude de demonstração", "Referencial sintético do cenário de demonstração"));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
