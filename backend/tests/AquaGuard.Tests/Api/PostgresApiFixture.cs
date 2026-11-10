using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AquaGuard.Domain.Entities;
using AquaGuard.Infrastructure.Persistence;
using Npgsql;
using Xunit;

namespace AquaGuard.Tests.Api;

[CollectionDefinition(Name)]
public sealed class PostgresApiCollection : ICollectionFixture<PostgresApiFixture>
{
    public const string Name = "PostgreSQL API integration";
}

public sealed class PostgresApiFixture : IDisposable
{
    public WebApplicationFactory<Program> Factory { get; }

    public PostgresApiFixture()
    {
        var connectionString = Environment.GetEnvironmentVariable("AQUAGUARD_TEST_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=aquaguard_test;Username=aquaguard;Password=aquaguard-local";
        var database = new NpgsqlConnectionStringBuilder(connectionString).Database ?? string.Empty;
        if (!database.EndsWith("_test", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Integration tests require a database whose name ends with '_test'.");
        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.UseSetting("ConnectionStrings:AquaGuard", connectionString);
            builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(
                new Dictionary<string, string?> { ["ConnectionStrings:AquaGuard"] = connectionString }));
        });
    }

    public async Task<Guid> CreateReservoirAsync()
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AquaGuardDbContext>();
        var id = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        dbContext.Properties.Add(new Property(propertyId, $"Integration test property {propertyId:N}"));
        dbContext.Reservoirs.Add(new Reservoir(id, propertyId,
            "Reservoir integration test", "Synthetic test reference"));
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        return id;
    }

    public void Dispose() => Factory.Dispose();
}
