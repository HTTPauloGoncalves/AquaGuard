using AquaGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AquaGuard.Infrastructure.Persistence;

public sealed class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("properties");
        builder.HasKey(property => property.Id);
        builder.Property(property => property.Id).ValueGeneratedNever();
        builder.Property(property => property.Name).HasMaxLength(160).IsRequired();
        builder.Property(property => property.CreatedAt).HasColumnType("timestamp with time zone");
        builder.Ignore(property => property.OwnerId);
        builder.Ignore(property => property.Description);
        builder.Ignore(property => property.Owner);
        builder.Ignore(property => property.Coordinate);
        builder.Ignore(property => property.Stations);
        builder.HasMany(property => property.Reservoirs).WithOne(reservoir => reservoir.Property)
            .HasForeignKey(reservoir => reservoir.PropertyId);
    }
}
