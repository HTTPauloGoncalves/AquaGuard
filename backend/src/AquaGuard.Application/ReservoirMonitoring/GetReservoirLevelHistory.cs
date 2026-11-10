using AquaGuard.Domain.Entities;
using AquaGuard.Domain.ReservoirMonitoring;

namespace AquaGuard.Application.ReservoirMonitoring;

public sealed record ReservoirLevelHistory(ReservoirSummary Reservoir, DateTimeOffset From,
    DateTimeOffset To, Measurement? LatestValid, IReadOnlyList<Measurement> History,
    LevelVariation Variation, IReadOnlyList<Measurement> Precipitation);

public sealed class GetReservoirLevelHistory(IReservoirMonitoringRepository repository)
{
    public async Task<ReservoirLevelHistory?> ExecuteAsync(Guid reservoirId, DateTimeOffset from,
        DateTimeOffset to, bool includePrecipitation, CancellationToken cancellationToken)
    {
        var reservoir = await repository.GetReservoirAsync(reservoirId, cancellationToken);
        if (reservoir is null) return null;
        var levels = await repository.GetValidLevelsAsync(reservoirId, from, to, cancellationToken);
        var precipitation = includePrecipitation
            ? await repository.GetValidPrecipitationAsync(reservoirId, from, to, cancellationToken)
            : Array.Empty<Measurement>();
        var variation = LevelVariationCalculator.Calculate(levels);
        return new ReservoirLevelHistory(reservoir, from, to,
            await repository.GetLatestValidLevelAsync(reservoirId, cancellationToken), levels, variation,
            precipitation);
    }
}
