using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace AquaGuard.Tests.Api;

[Collection(PostgresApiCollection.Name)]
public sealed class SimulatedMeasurementIngestionTests(PostgresApiFixture fixture)
{
    [Fact]
    public async Task Persists_receipt_and_returns_validation_state()
    {
        var reservoirId = await fixture.CreateReservoirAsync();
        var response = await PostAsync(Guid.NewGuid(), new
        {
            reservoirId, kind = "water_level", value = 2.45, unit = "m",
            measuredAt = DateTimeOffset.UtcNow
        });

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        using var result = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Equal("valid", result.RootElement.GetProperty("validationStatus").GetString());
    }

    [Fact]
    public async Task Semantically_incomplete_json_is_recorded_as_invalid()
    {
        var reservoirId = await fixture.CreateReservoirAsync();
        var response = await PostAsync(Guid.NewGuid(), new { reservoirId });

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        using var result = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Equal("invalid", result.RootElement.GetProperty("validationStatus").GetString());
    }

    [Fact]
    public async Task Returns_existing_receipt_for_duplicate_and_conflict_for_changed_payload()
    {
        var eventId = Guid.NewGuid();
        var reservoirId = await fixture.CreateReservoirAsync();
        var payload = new { reservoirId, kind = "water_level", value = 2.2, unit = "m", measuredAt = DateTimeOffset.UtcNow };
        Assert.Equal(HttpStatusCode.Accepted, (await PostAsync(eventId, payload)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await PostAsync(eventId, payload)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await PostAsync(eventId, new
        {
            reservoirId, kind = "water_level", value = 2.3, unit = "m", measuredAt = payload.measuredAt
        })).StatusCode);
    }

    [Fact]
    public async Task Late_received_measurement_does_not_replace_latest_by_measurement_time()
    {
        var baseTime = DateTimeOffset.UtcNow.AddDays(-3);
        var reservoirId = await fixture.CreateReservoirAsync();
        await PostAsync(Guid.NewGuid(), new { reservoirId, kind = "water_level", value = 2.4, unit = "m", measuredAt = baseTime.AddMinutes(2) });
        await PostAsync(Guid.NewGuid(), new { reservoirId, kind = "water_level", value = 2.1, unit = "m", measuredAt = baseTime });
        var response = await fixture.Factory.CreateClient().GetAsync($"/api/v1/reservoirs/{reservoirId}/level-monitoring?from={Uri.EscapeDataString(baseTime.AddHours(-1).ToString("O"))}&to={Uri.EscapeDataString(baseTime.AddHours(1).ToString("O"))}", TestContext.Current.CancellationToken);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Equal(2.4, document.RootElement.GetProperty("latestValid").GetProperty("value").GetDouble(), 6);
    }

    private async Task<HttpResponseMessage> PostAsync(Guid eventId, object payload)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/simulated-measurements")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Add("Idempotency-Key", eventId.ToString());
        return await fixture.Factory.CreateClient().SendAsync(request, TestContext.Current.CancellationToken);
    }
}
