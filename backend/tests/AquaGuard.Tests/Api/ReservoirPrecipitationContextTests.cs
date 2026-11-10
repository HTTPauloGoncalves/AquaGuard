using System.Text.Json;
using System.Net.Http.Json;
using Xunit;

namespace AquaGuard.Tests.Api;

[Collection(PostgresApiCollection.Name)]
public sealed class ReservoirPrecipitationContextTests(PostgresApiFixture fixture)
{
    [Fact]
    public async Task Optional_precipitation_preserves_zero_value_and_accumulation_period()
    {
        var end = DateTimeOffset.UtcNow.AddDays(-10);
        var start = end.AddHours(-1);
        var reservoirId = await fixture.CreateReservoirAsync();
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/simulated-measurements")
        {
            Content = JsonContent.Create(new { reservoirId, kind = "precipitation", value = 0, unit = "mm", measuredAt = end, accumulationStart = start, accumulationEnd = end })
        };
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
        (await fixture.Factory.CreateClient().SendAsync(request, TestContext.Current.CancellationToken)).EnsureSuccessStatusCode();

        var response = await fixture.Factory.CreateClient().GetAsync($"/api/v1/reservoirs/{reservoirId}/level-monitoring?from={Uri.EscapeDataString(start.AddMinutes(-1).ToString("O"))}&to={Uri.EscapeDataString(end.AddMinutes(1).ToString("O"))}&includePrecipitation=true", TestContext.Current.CancellationToken);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var rainfall = body.RootElement.GetProperty("precipitation").EnumerateArray().Single();
        Assert.Equal(0, rainfall.GetProperty("value").GetDouble());
        Assert.Equal("mm", rainfall.GetProperty("unit").GetString());
        var persistedStart = rainfall.GetProperty("accumulationStart").GetDateTimeOffset();
        Assert.InRange(Math.Abs((persistedStart - start).Ticks), 0L, 10L);
    }
}
