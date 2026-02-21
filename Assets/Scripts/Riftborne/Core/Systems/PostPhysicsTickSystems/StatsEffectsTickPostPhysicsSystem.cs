using Riftborne.Core.Stores;

namespace Riftborne.Core.Systems.PostPhysicsTickSystems
{
    public sealed class StatsEffectsTickPostPhysicsSystem : IPostPhysicsTickSystem
    {
        private readonly IStatsEffectStore _effects;

        public StatsEffectsTickPostPhysicsSystem(IStatsEffectStore effects)
        {
            _effects = effects;
        }

        public void Tick(int tick)
        {
            _effects.TickDurations();
        }
    }
}