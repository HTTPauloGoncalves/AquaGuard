using AquaGuard.Domain.Entities;
using AquaGuard.Domain.ReservoirMonitoring;
using Xunit;

namespace AquaGuard.Tests.ReservoirMonitoring;

public sealed class LevelHistoryQueryTests
{
    [Fact]
    public void Variation_ignores_invalid_readings_and_uses_distinct_measurement_times()
    {
        var firstAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z");
        var middleAt = firstAt.AddHours(1);
        var result = LevelVariationCalculator.Calculate(new[]
        {
            Measurement.ValidLevel(2.0, firstAt),
            Measurement.ValidLevel(2.8, firstAt),
            InvalidLevel(9.0, firstAt.AddMinutes(30)),
            Measurement.ValidLevel(2.5, middleAt)
        });

        Assert.Equal(0.5, result.DeltaMeters!.Value, 6);
        Assert.Equal(firstAt, result.StartMeasuredAt);
        Assert.Equal(middleAt, result.EndMeasuredAt);
    }

    [Fact]
    public void One_distinct_timestamp_has_no_variation()
    {
        var timestamp = DateTimeOffset.UtcNow;
        var result = LevelVariationCalculator.Calculate(new[]
        {
            Measurement.ValidLevel(2.2, timestamp), Measurement.ValidLevel(2.3, timestamp)
        });

        Assert.False(result.IsAvailable);
    }

    private static Measurement InvalidLevel(double value, DateTimeOffset measuredAt) => new(
        Guid.NewGuid(), Guid.NewGuid(), "{}", "water_level", value, value.ToString(), "m", measuredAt,
        null, null, measuredAt, new MeasurementValidationResult(false, ["invalid test reading"]));
}
