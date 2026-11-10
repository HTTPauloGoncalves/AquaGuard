namespace AquaGuard.Simulator.Simulation;

public sealed class ReservoirScenario(IConfiguration configuration)
{
    private static readonly double[] DefaultLevelSequence = [2.30, 2.34, 2.39, 2.35, 2.29, 2.27, 2.32];
    private static readonly double?[] DefaultRainSequence = [0, 0, 3.2, null, 0, 1.4, null];
    private readonly double[] _levelSequence = configuration.GetSection("Simulation:LevelSequenceMeters").Get<double[]>()
        is { Length: > 0 } levels ? levels : DefaultLevelSequence;
    private readonly double?[] _rainSequence = configuration.GetSection("Simulation:PrecipitationSequenceMm").Get<double?[]>()
        is { Length: > 0 } rainfall ? rainfall : DefaultRainSequence;

    public Guid ReservoirId { get; } = Guid.TryParse(configuration["Simulation:ReservoirId"], out var reservoirId)
        ? reservoirId : Guid.Parse("00000000-0000-0000-0000-000000000001");
    public int IntervalSeconds { get; } = Math.Clamp(configuration.GetValue("Simulation:IntervalSeconds", 60), 1, 3600);

    public (double Level, double? Precipitation) At(long cycle) =>
        (_levelSequence[(int)(cycle % _levelSequence.Length)],
            _rainSequence[(int)(cycle % _rainSequence.Length)]);
}
