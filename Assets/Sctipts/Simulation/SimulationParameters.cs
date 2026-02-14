namespace Game.Simulation
{

    public sealed class SimulationParameters
    {
        public float UnitsPerTick { get; }

        public SimulationParameters(float unitsPerTick)
        {
            UnitsPerTick = unitsPerTick;
        }
    }
}