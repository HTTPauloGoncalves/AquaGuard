using AquaGuard.Domain.ReservoirMonitoring;
using Xunit;

namespace AquaGuard.Tests.ReservoirMonitoring;

public sealed class PrecipitationMeasurementTests
{
    [Fact]
    public void Zero_is_a_valid_precipitation_value_when_period_and_unit_are_present()
    {
        var end = DateTimeOffset.UtcNow;
        var result = MeasurementValidator.Validate("precipitation", 0, "mm", end, end.AddHours(-1), end);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(null, "mm")]
    [InlineData(1.2, "m")]
    [InlineData(-0.1, "mm")]
    public void Missing_or_invalid_precipitation_is_not_treated_as_zero(double? value, string? unit)
    {
        var end = DateTimeOffset.UtcNow;
        var result = MeasurementValidator.Validate("precipitation", value, unit, end, end, end.AddMinutes(-1));

        Assert.False(result.IsValid);
    }
}
