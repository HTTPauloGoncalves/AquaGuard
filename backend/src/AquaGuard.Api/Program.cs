using Scalar.AspNetCore;
using AquaGuard.Api.Endpoints;
using AquaGuard.Infrastructure;
using AquaGuard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .WithOrigins(builder.Configuration["Frontend:Origin"] ?? "http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()));
builder.Services.AddAquaGuardInfrastructure(builder.Configuration);
builder.Services.AddScoped<AquaGuard.Application.ReservoirMonitoring.RecordSimulatedMeasurement>();
builder.Services.AddScoped<AquaGuard.Application.ReservoirMonitoring.GetLatestReservoirLevel>();
builder.Services.AddScoped<AquaGuard.Application.ReservoirMonitoring.GetReservoirLevelHistory>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AquaGuardDbContext>();
    await dbContext.Database.MigrateAsync();
    await DemoDataSeeder.SeedAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new HealthResponse("healthy")))
    .WithName("GetHealth")
    .WithTags("Health")
    .WithSummary("Verifica se a API está respondendo.");

var api = app.MapGroup("/api/v1")
    .WithTags("API v1");

api.MapReservoirMonitoring();

api.MapGet("/", () => Results.Ok(new ApiInfoResponse(
        "AquaGuard API",
        "v1",
        "API de monitoramento hídrico com dados simulados para o reservatório de demonstração.")))
    .WithName("GetApiInfo")
    .WithSummary("Retorna informações sobre a versão inicial da API.");

app.Run();

internal sealed record HealthResponse(string Status);

internal sealed record ApiInfoResponse(string Name, string Version, string Description);

public partial class Program { }
