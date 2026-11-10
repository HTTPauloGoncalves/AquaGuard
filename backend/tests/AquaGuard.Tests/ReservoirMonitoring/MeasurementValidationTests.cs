using AquaGuard.Domain.Entities;
using AquaGuard.Domain.ReservoirMonitoring;
using Xunit;

namespace AquaGuard.Tests.ReservoirMonitoring;

public sealed class MeasurementValidationTests
{
    [Theory]
    [InlineData(2.45, "m", true)]
    [InlineData(double.NaN, "m", false)]
    [InlineData(2.45, "cm", false)]
    [InlineData(2.45, null, false)]
    public void Level_measurement_requires_finite_value_expected_unit_and_timestamp(
        double value,
        string? unit,
        bool expectedValid)
    {
        var result = MeasurementValidator.Validate(
            "water_level", value, unit, DateTimeOffset.UtcNow, null, null);

        Assert.Equal(expectedValid, result.IsValid);
    }

    [Fact]
    public void Unknown_kind_is_invalid()
    {
        var result = MeasurementValidator.Validate(
            "temperature", 20, "C", DateTimeOffset.UtcNow, null, null);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Precipitation_requires_a_positive_accumulation_interval()
    {
        var end = DateTimeOffset.UtcNow;
        var result = MeasurementValidator.Validate("precipitation", 0, "mm", end,
            end.AddHours(-1), end);

        Assert.True(result.IsValid);
    }
}
