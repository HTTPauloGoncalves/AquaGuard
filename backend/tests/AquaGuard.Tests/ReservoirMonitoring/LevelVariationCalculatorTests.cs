using AquaGuard.Domain.Entities;
using AquaGuard.Domain.ReservoirMonitoring;
using Xunit;

namespace AquaGuard.Tests.ReservoirMonitoring;

public sealed class LevelVariationCalculatorTests
{
    [Theory]
    [InlineData(2.3, 2.45, 0.15, LevelVariationDirection.Rising)]
    [InlineData(2.45, 2.3, -0.15, LevelVariationDirection.Falling)]
    [InlineData(2.3, 2.3, 0, LevelVariationDirection.NoObservedChange)]
    public void Calculates_delta_and_direction(double start, double end, double expected,
        LevelVariationDirection direction)
    {
        var startAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z");
        var endAt = startAt.AddMinutes(1);
        var result = LevelVariationCalculator.Calculate(new[]
        {
            Measurement.ValidLevel(start, startAt),
            Measurement.ValidLevel(end, endAt)
        });

        Assert.Equal(expected, result.DeltaMeters!.Value, 6);
        Assert.Equal(direction, result.Direction);
        Assert.Equal(startAt, result.StartMeasuredAt);
        Assert.Equal(endAt, result.EndMeasuredAt);
    }

    [Fact]
    public void Variation_is_unavailable_for_fewer_than_two_distinct_timestamps()
    {
        var timestamp = DateTimeOffset.UtcNow;
        var result = LevelVariationCalculator.Calculate(new[]
        {
            Measurement.ValidLevel(2.3, timestamp),
            Measurement.ValidLevel(2.4, timestamp)
        });

        Assert.False(result.IsAvailable);
    }
}
