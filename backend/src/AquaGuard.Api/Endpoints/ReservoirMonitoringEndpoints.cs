using System.Globalization;
using System.Text.Json;
using AquaGuard.Application.ReservoirMonitoring;
using AquaGuard.Domain.Entities;
using AquaGuard.Domain.ReservoirMonitoring;

namespace AquaGuard.Api.Endpoints;

public static class ReservoirMonitoringEndpoints
{
    public static RouteGroupBuilder MapReservoirMonitoring(this RouteGroupBuilder api)
    {
        api.MapPost("/simulated-measurements", ReceiveMeasurement)
            .WithName("ReceiveSimulatedMeasurement");
        api.MapGet("/reservoirs/{reservoirId:guid}/level-monitoring", GetMonitoring)
            .WithName("GetReservoirLevelMonitoring");
        return api;
    }

    private static async Task<IResult> ReceiveMeasurement(HttpRequest request,
        RecordSimulatedMeasurement recordMeasurement, ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("ReservoirMonitoring.Ingestion");
        if (!Guid.TryParse(request.Headers["Idempotency-Key"], out var eventId))
            return Results.BadRequest(new { error = "A valid Idempotency-Key UUID is required." });

        JsonDocument document;
        try { document = await JsonDocument.ParseAsync(request.Body, cancellationToken: cancellationToken); }
        catch (JsonException) { return Results.BadRequest(new { error = "Request body must be valid JSON." }); }

        using (document)
        {
            if (document.RootElement.ValueKind != JsonValueKind.Object) return Results.BadRequest(new { error = "Request body must be a JSON object." });
            var root = document.RootElement;
            if (!root.TryGetProperty("reservoirId", out var reservoirProperty)
                || !Guid.TryParse(reservoirProperty.GetString(), out var reservoirId))
                return Results.BadRequest(new { error = "A valid reservoirId UUID is required." });

            var kind = ReadString(root, "kind");
            var unit = ReadString(root, "unit");
            var valueElement = root.TryGetProperty("value", out var value) ? value : default;
            var rawValue = valueElement.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? null : valueElement.GetRawText();
            double? numericValue = valueElement.ValueKind == JsonValueKind.Number && valueElement.TryGetDouble(out var parsedValue)
                ? parsedValue : null;
            var measuredAt = ReadTimestamp(root, "measuredAt");
            var accumulationStart = ReadTimestamp(root, "accumulationStart");
            var accumulationEnd = ReadTimestamp(root, "accumulationEnd");
            var payload = root.GetRawText();
            var input = new SimulatedMeasurementInput(reservoirId, kind, numericValue, rawValue, unit,
                measuredAt, accumulationStart, accumulationEnd, payload);
            var receipt = await recordMeasurement.ExecuteAsync(eventId, input, cancellationToken);
            if (receipt is null) return Results.NotFound(new { error = "Reservoir not found." });
            if (receipt.IsConflict) return Results.Conflict(new { error = "Idempotency key was already used with a different payload." });
            if (receipt.ValidationStatus == "invalid")
                logger.LogWarning("Simulated measurement {EventId} for reservoir {ReservoirId} was received as invalid: {Reasons}",
                    eventId, reservoirId, string.Join("; ", receipt.ValidationMessages));
            else
                logger.LogInformation("Simulated measurement {EventId} for reservoir {ReservoirId} was recorded.",
                    eventId, reservoirId);
            var response = new
            {
                receiptId = receipt.ReceiptId,
                sourceEventId = receipt.SourceEventId,
                source = receipt.Source,
                receivedAt = receipt.ReceivedAt,
                validationStatus = receipt.ValidationStatus,
                validationMessages = receipt.ValidationMessages
            };
            return receipt.IsDuplicate ? Results.Ok(response) : Results.Accepted(value: response);
        }
    }

    private static async Task<IResult> GetMonitoring(Guid reservoirId, string? from, string? to,
        bool? includePrecipitation, GetReservoirLevelHistory getHistory, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var end = to is null ? now : ParseTimestamp(to);
        var start = from is null ? end?.AddHours(-24) : ParseTimestamp(from);
        if (start is null || end is null || start > end)
            return Results.BadRequest(new { error = "from/to must be valid ISO 8601 timestamps and from must be <= to." });

        var result = await getHistory.ExecuteAsync(reservoirId, start.Value, end.Value,
            includePrecipitation == true, cancellationToken);
        if (result is null) return Results.NotFound(new { error = "Reservoir not found." });
        return Results.Ok(new
        {
            reservoirId,
            reservoirName = result.Reservoir.Name,
            levelReferenceLabel = result.Reservoir.LevelReferenceLabel,
            window = new { from = result.From, to = result.To },
            latestValid = MapLevel(result.LatestValid),
            history = result.History.Select(MapLevel),
            variation = result.Variation.IsAvailable ? new
            {
                deltaMeters = result.Variation.DeltaMeters,
                startMeasuredAt = result.Variation.StartMeasuredAt,
                endMeasuredAt = result.Variation.EndMeasuredAt,
                direction = result.Variation.Direction switch
                {
                    LevelVariationDirection.Rising => "rising",
                    LevelVariationDirection.Falling => "falling",
                    _ => "no_observed_change"
                }
            } : null,
            precipitation = includePrecipitation == true
                ? result.Precipitation.Select(MapPrecipitation)
                : null
        });
    }

    private static object? MapLevel(Measurement? measurement) => measurement is null ? null : new
    {
        value = measurement.NumericValue,
        unit = "m",
        measuredAt = measurement.MeasuredAt,
        source = measurement.Source
    };

    private static object MapPrecipitation(Measurement measurement) => new
    {
        value = measurement.NumericValue,
        unit = "mm",
        measuredAt = measurement.MeasuredAt,
        accumulationStart = measurement.AccumulationStart,
        accumulationEnd = measurement.AccumulationEnd,
        source = measurement.Source
    };

    private static string? ReadString(JsonElement root, string name) =>
        root.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    private static DateTimeOffset? ReadTimestamp(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var value) || value.ValueKind != JsonValueKind.String) return null;
        var raw = value.GetString();
        return raw is not null && HasUtcOffset(raw)
            && DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var timestamp) ? timestamp.ToUniversalTime() : null;
    }

    private static DateTimeOffset? ParseTimestamp(string value) => HasUtcOffset(value)
        && DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None,
            out var timestamp) ? timestamp.ToUniversalTime() : null;

    private static bool HasUtcOffset(string value)
    {
        if (value.EndsWith("Z", StringComparison.OrdinalIgnoreCase)) return true;
        var timeSeparator = value.IndexOf('T');
        return timeSeparator >= 0 && (value.IndexOf('+', timeSeparator) >= 0 || value.LastIndexOf('-') > timeSeparator);
    }
}
