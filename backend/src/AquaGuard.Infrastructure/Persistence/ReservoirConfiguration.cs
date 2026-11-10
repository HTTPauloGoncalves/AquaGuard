using AquaGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AquaGuard.Infrastructure.Persistence;

public sealed class ReservoirConfiguration : IEntityTypeConfiguration<Reservoir>
{
    public void Configure(EntityTypeBuilder<Reservoir> builder)
    {
        builder.ToTable("reservoirs");
        builder.HasKey(reservoir => reservoir.Id);
        builder.Property(reservoir => reservoir.Id).ValueGeneratedNever();
        builder.Property(reservoir => reservoir.PropertyId).IsRequired();
        builder.Property(reservoir => reservoir.Name).HasMaxLength(160).IsRequired();
        builder.Property(reservoir => reservoir.LevelReferenceLabel).HasMaxLength(200).IsRequired();
        builder.Property(reservoir => reservoir.CreatedAt).HasColumnType("timestamp with time zone");
        builder.HasIndex(reservoir => reservoir.PropertyId).IsUnique();
    }
}
