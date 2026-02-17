using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Model;

namespace Riftborne.Core.Stores
{
    public sealed class MotorStateStore : IMotorStateStore
    {
        private readonly Dictionary<GameEntityId, MotorState> _map = new();

        public MotorState GetOrCreate(GameEntityId id)
        {
            if (_map.TryGetValue(id, out var s))
                return s;

            s = new MotorState(id);
            _map[id] = s;
            return s;
        }

        public void Set(MotorState state)
        {
            _map[state.EntityId] = state;
        }

        public void Remove(GameEntityId id)
        {
            _map.Remove(id);
        }
    }
}