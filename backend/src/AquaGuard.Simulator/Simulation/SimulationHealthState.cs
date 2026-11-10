namespace AquaGuard.Simulator.Simulation;

public sealed class SimulationHealthState
{
    public bool IsRunning { get; private set; }
    public void MarkStarted() => IsRunning = true;
    public void MarkStopped() => IsRunning = false;
}
