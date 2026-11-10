using AquaGuard.Domain.Entities;
using AquaGuard.Domain.ReservoirMonitoring;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AquaGuard.Application.ReservoirMonitoring;

public sealed record SimulatedMeasurementInput(Guid ReservoirId, string? Kind, double? Value,
    string? RawValue, string? Unit, DateTimeOffset? MeasuredAt, DateTimeOffset? AccumulationStart,
    DateTimeOffset? AccumulationEnd, string RawPayload);

public sealed record MeasurementReceipt(Guid ReceiptId, Guid SourceEventId, string Source,
    DateTimeOffset ReceivedAt, string ValidationStatus, IReadOnlyList<string> ValidationMessages,
    bool IsDuplicate = false, bool IsConflict = false);

public sealed class RecordSimulatedMeasurement(IReservoirMonitoringRepository repository)
{
    public async Task<MeasurementReceipt?> ExecuteAsync(Guid sourceEventId, SimulatedMeasurementInput input,
        CancellationToken cancellationToken)
    {
        var existing = await repository.FindBySourceEventIdAsync(sourceEventId, cancellationToken);
        if (existing is not null)
        {
            var samePayload = PayloadsMatch(existing.RawPayload, input.RawPayload);
            return new MeasurementReceipt(existing.Id, sourceEventId, existing.Source, existing.ReceivedAt,
                existing.ValidationStatus, System.Text.Json.JsonSerializer.Deserialize<string[]>(existing.ValidationMessages) ?? [],
                IsDuplicate: samePayload, IsConflict: !samePayload);
        }

        if (!await repository.ReservoirExistsAsync(input.ReservoirId, cancellationToken)) return null;
        var kind = input.Kind ?? string.Empty;
        var validation = MeasurementValidator.Validate(kind, input.Value, input.Unit, input.MeasuredAt,
            input.AccumulationStart, input.AccumulationEnd);
        var measurement = new Measurement(sourceEventId, input.ReservoirId, input.RawPayload, kind,
            input.Value, input.RawValue, input.Unit, input.MeasuredAt, input.AccumulationStart,
            input.AccumulationEnd, DateTimeOffset.UtcNow, validation);
        await repository.AddAsync(measurement, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return new MeasurementReceipt(measurement.Id, sourceEventId, measurement.Source,
            measurement.ReceivedAt, measurement.ValidationStatus, validation.Messages);
    }

    private static bool PayloadsMatch(string existingPayload, string incomingPayload)
    {
        try
        {
            return JsonNode.DeepEquals(JsonNode.Parse(existingPayload), JsonNode.Parse(incomingPayload));
        }
        catch (JsonException)
        {
            return existingPayload == incomingPayload;
        }
    }
}
