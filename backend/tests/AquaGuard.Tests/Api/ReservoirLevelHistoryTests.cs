using System.Text.Json;
using System.Net.Http.Json;
using Xunit;

namespace AquaGuard.Tests.Api;

[Collection(PostgresApiCollection.Name)]
public sealed class ReservoirLevelHistoryTests(PostgresApiFixture fixture)
{
    [Fact]
    public async Task History_is_inclusive_ordered_and_empty_period_is_supported()
    {
        var from = DateTimeOffset.UtcNow.AddDays(-5);
        var point = from.AddHours(1);
        var reservoirId = await fixture.CreateReservoirAsync();
        await PostAsync(reservoirId, new { reservoirId, kind = "water_level", value = 2.0, unit = "m", measuredAt = point });
        await PostAsync(reservoirId, new { reservoirId, kind = "water_level", value = 2.5, unit = "m", measuredAt = point.AddHours(1) });
        var client = fixture.Factory.CreateClient();
        var response = await client.GetAsync($"/api/v1/reservoirs/{reservoirId}/level-monitoring?from={Uri.EscapeDataString(point.ToString("O"))}&to={Uri.EscapeDataString(point.AddHours(1).ToString("O"))}", TestContext.Current.CancellationToken);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var history = body.RootElement.GetProperty("history");
        Assert.Equal(2, history.GetArrayLength());
        Assert.Equal(0.5, body.RootElement.GetProperty("variation").GetProperty("deltaMeters").GetDouble(), 6);

        var empty = await client.GetAsync($"/api/v1/reservoirs/{reservoirId}/level-monitoring?from={Uri.EscapeDataString(from.AddYears(-2).ToString("O"))}&to={Uri.EscapeDataString(from.AddYears(-1).ToString("O"))}", TestContext.Current.CancellationToken);
        using var emptyBody = JsonDocument.Parse(await empty.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Empty(emptyBody.RootElement.GetProperty("history").EnumerateArray());
        Assert.Equal(JsonValueKind.Null, emptyBody.RootElement.GetProperty("variation").ValueKind);
    }

    private async Task PostAsync(Guid reservoirId, object payload)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/simulated-measurements") { Content = JsonContent.Create(payload) };
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
        var response = await fixture.Factory.CreateClient().SendAsync(request, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
