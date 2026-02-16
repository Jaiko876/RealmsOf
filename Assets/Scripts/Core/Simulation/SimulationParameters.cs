namespace Game.Core.Simulation
{
    /// <summary>
    /// Parameters shared by simulation systems. Runtime-configured from GameConfig.
    /// </summary>
    public sealed class SimulationParameters
    {
        public float UnitsPerTick { get; }     // fallback for non-physical movement
        public float TickDeltaTime { get; }    // dt for physics & time-based systems
        public int PhysicsSubsteps { get; }    // 1..8

        public SimulationParameters(float unitsPerTick, float tickDeltaTime, int physicsSubsteps = 1)
        {
            UnitsPerTick = unitsPerTick;
            TickDeltaTime = tickDeltaTime;
            PhysicsSubsteps = physicsSubsteps < 1 ? 1 : physicsSubsteps;
        }
    }
}
