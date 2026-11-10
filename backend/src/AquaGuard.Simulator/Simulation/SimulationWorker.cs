using System.Net.Http.Json;

namespace AquaGuard.Simulator.Simulation;

public sealed class SimulationWorker(HttpClient httpClient, ReservoirScenario scenario,
    SimulationHealthState healthState, ILogger<SimulationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        healthState.MarkStarted();
        long cycle = 0;
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTimeOffset.UtcNow;
                var (level, precipitation) = scenario.At(cycle++);
                var levelPayload = new { reservoirId = scenario.ReservoirId, kind = "water_level", value = level,
                    unit = "m", measuredAt = now };
                var success = await SendWithRetriesAsync(levelPayload, stoppingToken);
                if (success && precipitation.HasValue)
                {
                    var start = now.AddSeconds(-scenario.IntervalSeconds);
                    var rainPayload = new { reservoirId = scenario.ReservoirId, kind = "precipitation",
                        value = precipitation.Value, unit = "mm", measuredAt = now,
                        accumulationStart = start, accumulationEnd = now };
                    await SendWithRetriesAsync(rainPayload, stoppingToken);
                }

                try { await Task.Delay(TimeSpan.FromSeconds(scenario.IntervalSeconds), stoppingToken); }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            }
        }
        finally
        {
            healthState.MarkStopped();
        }
    }

    private async Task<bool> SendWithRetriesAsync<T>(T payload, CancellationToken cancellationToken)
    {
        var eventId = Guid.NewGuid();
        for (var attempt = 0; attempt < 4; attempt++)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/simulated-measurements")
                {
                    Content = JsonContent.Create(payload)
                };
                request.Headers.Add("Idempotency-Key", eventId.ToString());
                using var response = await httpClient.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode) return true;
                logger.LogWarning("Simulator ingestion returned {StatusCode} on attempt {Attempt}.",
                    (int)response.StatusCode, attempt + 1);
                if ((int)response.StatusCode is >= 400 and < 500) return false;
            }
            catch (HttpRequestException exception)
            {
                logger.LogWarning(exception, "Simulator could not reach the API on attempt {Attempt}.", attempt + 1);
            }

            if (attempt < 3)
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                await Task.Delay(delay, cancellationToken);
            }
        }
        return false;
    }
}
