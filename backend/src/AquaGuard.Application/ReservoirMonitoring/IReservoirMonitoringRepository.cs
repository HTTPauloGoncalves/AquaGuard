using AquaGuard.Domain.Entities;

namespace AquaGuard.Application.ReservoirMonitoring;

public sealed record ReservoirSummary(Guid Id, string Name, string LevelReferenceLabel);

public interface IReservoirMonitoringRepository
{
    Task<bool> ReservoirExistsAsync(Guid reservoirId, CancellationToken cancellationToken);
    Task<Measurement?> FindBySourceEventIdAsync(Guid sourceEventId, CancellationToken cancellationToken);
    Task AddAsync(Measurement measurement, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task<ReservoirSummary?> GetReservoirAsync(Guid reservoirId, CancellationToken cancellationToken);
    Task<Measurement?> GetLatestValidLevelAsync(Guid reservoirId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Measurement>> GetValidLevelsAsync(Guid reservoirId, DateTimeOffset from,
        DateTimeOffset to, CancellationToken cancellationToken);
    Task<IReadOnlyList<Measurement>> GetValidPrecipitationAsync(Guid reservoirId, DateTimeOffset from,
        DateTimeOffset to, CancellationToken cancellationToken);
}
