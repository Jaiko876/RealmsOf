namespace Game.Presentation.Bootstrap
{

    public sealed class TickSettings
    {
        public int TickRate { get; }
        public int TickMs => 1000 / TickRate;

        public TickSettings(int tickRate) => TickRate = tickRate;
    }
}