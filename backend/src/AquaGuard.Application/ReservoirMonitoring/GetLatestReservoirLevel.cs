using AquaGuard.Domain.Entities;

namespace AquaGuard.Application.ReservoirMonitoring;

public sealed class GetLatestReservoirLevel(IReservoirMonitoringRepository repository)
{
    public Task<Measurement?> ExecuteAsync(Guid reservoirId, CancellationToken cancellationToken) =>
        repository.GetLatestValidLevelAsync(reservoirId, cancellationToken);
}
