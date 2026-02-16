using Riftborne.Core.Stats;

namespace Riftborne.Core.Combat.Health
{
    public interface IHealthTickSystem
    {
        void Tick(int tick, float dt);
    }

    public sealed class HealthTickSystem : IHealthTickSystem
    {
        private readonly IHealthStore _health;
        private readonly StatResolver _stats;

        public HealthTickSystem(IHealthStore health, StatResolver stats)
        {
            _health = health;
            _stats = stats;
        }

        public void Tick(int tick, float dt)
        {
            // В v1: реген применяем только к тем, у кого есть HealthState.
            // Итерации по всем сущностям пока нет. Это ок.
            // Если нужно — позже заменим store на enumerable/или хранение в GameState.
        }

        // Позже: когда появится список entityId с health — добавим foreach и regen:
        // regen = stats.Get(entityId, StatId.HpRegenPerSec) * dt;
        // hp = min(maxHp, hp + regen);
    }
}
