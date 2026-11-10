using AquaGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AquaGuard.Infrastructure.Persistence;

public sealed class MeasurementConfiguration : IEntityTypeConfiguration<Measurement>
{
    public void Configure(EntityTypeBuilder<Measurement> builder)
    {
        builder.ToTable("measurement_receipts");
        builder.HasKey(measurement => measurement.Id);
        builder.Property(measurement => measurement.Id).ValueGeneratedNever();
        builder.Property(measurement => measurement.SourceEventId).IsRequired();
        builder.HasIndex(measurement => measurement.SourceEventId).IsUnique();
        builder.Property(measurement => measurement.ReservoirId).IsRequired();
        builder.HasOne(measurement => measurement.Reservoir).WithMany().HasForeignKey(measurement => measurement.ReservoirId);
        builder.Property(measurement => measurement.Source).HasMaxLength(32).IsRequired();
        builder.Property(measurement => measurement.RawPayload).HasColumnType("jsonb").IsRequired();
        builder.Property(measurement => measurement.Kind).HasMaxLength(64).IsRequired();
        builder.Property(measurement => measurement.NumericValue).HasColumnType("double precision");
        builder.Property(measurement => measurement.RawValue).HasColumnType("text");
        builder.Property(measurement => measurement.Unit).HasMaxLength(16);
        builder.Property(measurement => measurement.MeasuredAt).HasColumnType("timestamp with time zone");
        builder.Property(measurement => measurement.ReceivedAt).HasColumnType("timestamp with time zone");
        builder.Property(measurement => measurement.AccumulationStart).HasColumnType("timestamp with time zone");
        builder.Property(measurement => measurement.AccumulationEnd).HasColumnType("timestamp with time zone");
        builder.Property(measurement => measurement.ValidationStatus).HasMaxLength(16).IsRequired();
        builder.Property(measurement => measurement.ValidationMessages).HasColumnType("jsonb").IsRequired();
        builder.Ignore(measurement => measurement.IsValid);
        builder.Ignore(measurement => measurement.IsLevel);
        builder.Ignore(measurement => measurement.IsPrecipitation);
        builder.HasIndex(measurement => new { measurement.ReservoirId, measurement.Kind, measurement.MeasuredAt });
    }
}
