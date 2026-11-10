using System.Text.Json;
using AquaGuard.Domain.ReservoirMonitoring;

namespace AquaGuard.Domain.Entities;

public sealed class Measurement
{
    private Measurement() { }

    public Measurement(Guid sourceEventId, Guid reservoirId, string rawPayload, string kind,
        double? numericValue, string? rawValue, string? unit, DateTimeOffset? measuredAt,
        DateTimeOffset? accumulationStart, DateTimeOffset? accumulationEnd, DateTimeOffset receivedAt,
        MeasurementValidationResult validation)
    {
        Id = Guid.NewGuid();
        SourceEventId = sourceEventId;
        ReservoirId = reservoirId;
        Source = "simulated";
        RawPayload = rawPayload;
        Kind = kind;
        NumericValue = validation.IsValid ? numericValue : null;
        RawValue = rawValue;
        Unit = unit;
        MeasuredAt = measuredAt?.ToUniversalTime();
        AccumulationStart = accumulationStart?.ToUniversalTime();
        AccumulationEnd = accumulationEnd?.ToUniversalTime();
        ReceivedAt = receivedAt.ToUniversalTime();
        ValidationStatus = validation.IsValid ? "valid" : "invalid";
        ValidationMessages = JsonSerializer.Serialize(validation.Messages);
    }

    public Guid Id { get; private set; }
    public Guid SourceEventId { get; private set; }
    public Guid ReservoirId { get; private set; }
    public Reservoir? Reservoir { get; private set; }
    public string Source { get; private set; } = "simulated";
    public string RawPayload { get; private set; } = "{}";
    public string Kind { get; private set; } = string.Empty;
    public double? NumericValue { get; private set; }
    public string? RawValue { get; private set; }
    public string? Unit { get; private set; }
    public DateTimeOffset? MeasuredAt { get; private set; }
    public DateTimeOffset ReceivedAt { get; private set; }
    public DateTimeOffset? AccumulationStart { get; private set; }
    public DateTimeOffset? AccumulationEnd { get; private set; }
    public string ValidationStatus { get; private set; } = "invalid";
    public string ValidationMessages { get; private set; } = "[]";
    public bool IsValid => ValidationStatus == "valid";
    public bool IsLevel => Kind == "water_level";
    public bool IsPrecipitation => Kind == "precipitation";

    public static Measurement ValidLevel(double value, DateTimeOffset measuredAt)
    {
        var validation = new MeasurementValidationResult(true, Array.Empty<string>());
        return new Measurement(Guid.NewGuid(), Guid.NewGuid(), "{}", "water_level", value,
            value.ToString(System.Globalization.CultureInfo.InvariantCulture), "m", measuredAt,
            null, null, measuredAt, validation);
    }
}
