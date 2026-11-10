using AquaGuard.Application.ReservoirMonitoring;
using AquaGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquaGuard.Infrastructure.Persistence;

public sealed class ReservoirMonitoringRepository(AquaGuardDbContext dbContext) : IReservoirMonitoringRepository
{
    public Task<bool> ReservoirExistsAsync(Guid reservoirId, CancellationToken cancellationToken) =>
        dbContext.Reservoirs.AnyAsync(reservoir => reservoir.Id == reservoirId, cancellationToken);

    public Task<Measurement?> FindBySourceEventIdAsync(Guid sourceEventId, CancellationToken cancellationToken) =>
        dbContext.Measurements.SingleOrDefaultAsync(measurement => measurement.SourceEventId == sourceEventId,
            cancellationToken);

    public async Task AddAsync(Measurement measurement, CancellationToken cancellationToken) =>
        await dbContext.Measurements.AddAsync(measurement, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);

    public Task<ReservoirSummary?> GetReservoirAsync(Guid reservoirId, CancellationToken cancellationToken) =>
        dbContext.Reservoirs.AsNoTracking()
            .Where(reservoir => reservoir.Id == reservoirId)
            .Select(reservoir => new ReservoirSummary(reservoir.Id, reservoir.Name, reservoir.LevelReferenceLabel))
            .SingleOrDefaultAsync(cancellationToken);

    public Task<Measurement?> GetLatestValidLevelAsync(Guid reservoirId, CancellationToken cancellationToken) =>
        dbContext.Measurements.AsNoTracking()
            .Where(measurement => measurement.ReservoirId == reservoirId && measurement.Kind == "water_level"
                && measurement.ValidationStatus == "valid" && measurement.MeasuredAt != null)
            .OrderByDescending(measurement => measurement.MeasuredAt)
            .ThenByDescending(measurement => measurement.ReceivedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<Measurement>> GetValidLevelsAsync(Guid reservoirId, DateTimeOffset from,
        DateTimeOffset to, CancellationToken cancellationToken) => await dbContext.Measurements.AsNoTracking()
        .Where(measurement => measurement.ReservoirId == reservoirId && measurement.Kind == "water_level"
            && measurement.ValidationStatus == "valid" && measurement.MeasuredAt >= from && measurement.MeasuredAt <= to)
        .OrderBy(measurement => measurement.MeasuredAt)
        .ThenBy(measurement => measurement.ReceivedAt)
        .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Measurement>> GetValidPrecipitationAsync(Guid reservoirId,
        DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken) =>
        await dbContext.Measurements.AsNoTracking()
            .Where(measurement => measurement.ReservoirId == reservoirId && measurement.Kind == "precipitation"
                && measurement.ValidationStatus == "valid" && measurement.AccumulationStart <= to
                && measurement.AccumulationEnd >= from && measurement.AccumulationStart != null
                && measurement.AccumulationEnd != null)
            .OrderBy(measurement => measurement.AccumulationEnd)
            .ToListAsync(cancellationToken);
}
