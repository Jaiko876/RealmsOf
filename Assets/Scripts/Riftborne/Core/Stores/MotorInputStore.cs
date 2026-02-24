using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.Core.Stores
{
    public sealed class MotorInputStore : IMotorInputStore
    {
        private readonly Dictionary<GameEntityId, MotorInput> _inputs = new();

        public void Set(GameEntityId id, MotorInput input) => _inputs[id] = input;

        public bool TryGet(GameEntityId id, out MotorInput input) => _inputs.TryGetValue(id, out input);

        public void Clear() => _inputs.Clear();
    }
}