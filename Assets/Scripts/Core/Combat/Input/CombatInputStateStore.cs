using System.Collections.Generic;
using Game.Core.Model;

namespace Game.Core.Combat.Input
{
    public sealed class CombatInputStateStore
    {
        private sealed class EntityState
        {
            public bool AttackHeld;
            public int AttackPressTick;

            public bool DefenseHeld;
            public int DefensePressTick;

            public readonly List<BufferedIntent> Buffer = new List<BufferedIntent>(8);
        }

        private readonly Dictionary<GameEntityId, EntityState> _map = new Dictionary<GameEntityId, EntityState>();

        private EntityState Get(GameEntityId id)
        {
            if (!_map.TryGetValue(id, out var s))
            {
                s = new EntityState();
                _map[id] = s;
            }
            return s;
        }

        public void OnAttackPress(int tick, GameEntityId id)
        {
            var s = Get(id);
            s.AttackHeld = true;
            s.AttackPressTick = tick;
        }

        public int OnAttackRelease(int tick, GameEntityId id)
        {
            var s = Get(id);
            if (!s.AttackHeld)
                return 0;

            s.AttackHeld = false;
            return tick - s.AttackPressTick;
        }

        public void OnDefensePress(int tick, GameEntityId id)
        {
            var s = Get(id);
            s.DefenseHeld = true;
            s.DefensePressTick = tick;
        }

        public int OnDefenseRelease(int tick, GameEntityId id)
        {
            var s = Get(id);
            if (!s.DefenseHeld)
                return 0;

            s.DefenseHeld = false;
            return tick - s.DefensePressTick;
        }

        public void Enqueue(GameEntityId id, BufferedIntent intent)
        {
            Get(id).Buffer.Add(intent);
        }

        public List<BufferedIntent> GetBuffer(GameEntityId id)
        {
            return Get(id).Buffer;
        }
    }
}
