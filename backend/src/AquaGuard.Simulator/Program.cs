using AquaGuard.Simulator.Simulation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ReservoirScenario>();
builder.Services.AddSingleton<SimulationHealthState>();
builder.Services.AddHttpClient<SimulationWorker>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"] ?? "http://localhost:5226");
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddHostedService(serviceProvider => serviceProvider.GetRequiredService<SimulationWorker>());

var app = builder.Build();
app.MapGet("/health", (SimulationHealthState state) =>
{
    return state.IsRunning ? Results.Ok(new { status = "healthy" })
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
});
app.Run();
