namespace AquaGuard.Domain.ReservoirMonitoring;

public sealed record MeasurementValidationResult(bool IsValid, IReadOnlyList<string> Messages);

public static class MeasurementValidator
{
    public static MeasurementValidationResult Validate(string kind, double? value, string? unit,
        DateTimeOffset? measuredAt, DateTimeOffset? accumulationStart,
        DateTimeOffset? accumulationEnd)
    {
        var errors = new List<string>();
        if (kind is not ("water_level" or "precipitation")) errors.Add("Unknown measurement kind.");
        if (value is null || !double.IsFinite(value.Value)) errors.Add("Value must be a finite number.");
        var expectedUnit = kind == "water_level" ? "m" : kind == "precipitation" ? "mm" : null;
        if (expectedUnit is null || unit != expectedUnit) errors.Add("Unit is missing or incompatible.");
        if (measuredAt is null) errors.Add("Measurement time is missing or invalid.");
        if (kind == "precipitation")
        {
            if (value < 0) errors.Add("Precipitation cannot be negative.");
            if (accumulationStart is null || accumulationEnd is null || accumulationEnd <= accumulationStart)
                errors.Add("Precipitation accumulation interval is missing or invalid.");
        }
        return new MeasurementValidationResult(errors.Count == 0, errors);
    }
}
