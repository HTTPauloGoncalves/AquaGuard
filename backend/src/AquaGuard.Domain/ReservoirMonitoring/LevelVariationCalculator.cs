using AquaGuard.Domain.Entities;

namespace AquaGuard.Domain.ReservoirMonitoring;

public enum LevelVariationDirection { Rising, Falling, NoObservedChange }

public sealed record LevelVariation(bool IsAvailable, double? DeltaMeters,
    DateTimeOffset? StartMeasuredAt, DateTimeOffset? EndMeasuredAt,
    LevelVariationDirection? Direction)
{
    public static LevelVariation Unavailable { get; } = new(false, null, null, null, null);
}

public static class LevelVariationCalculator
{
    public static LevelVariation Calculate(IEnumerable<Measurement> measurements)
    {
        var points = measurements
            .Where(m => m.IsValid && m.IsLevel && m.NumericValue.HasValue && m.MeasuredAt.HasValue)
            .GroupBy(m => m.MeasuredAt!.Value)
            .Select(group => group.First())
            .OrderBy(m => m.MeasuredAt)
            .ToArray();
        if (points.Length < 2) return LevelVariation.Unavailable;

        var first = points[0];
        var last = points[^1];
        var delta = last.NumericValue!.Value - first.NumericValue!.Value;
        if (!double.IsFinite(delta)) return LevelVariation.Unavailable;
        var direction = delta > 0 ? LevelVariationDirection.Rising
            : delta < 0 ? LevelVariationDirection.Falling
            : LevelVariationDirection.NoObservedChange;
        return new LevelVariation(true, delta, first.MeasuredAt, last.MeasuredAt, direction);
    }
}
