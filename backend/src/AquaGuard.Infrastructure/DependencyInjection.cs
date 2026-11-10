using AquaGuard.Application.ReservoirMonitoring;
using AquaGuard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AquaGuard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAquaGuardInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AquaGuard")
            ?? throw new InvalidOperationException("Connection string 'AquaGuard' is required.");
        services.AddDbContext<AquaGuardDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IReservoirMonitoringRepository, ReservoirMonitoringRepository>();
        return services;
    }
}
