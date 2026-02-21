using Riftborne.Core.Commands;
using Riftborne.Core.Model;

namespace Riftborne.Core.Spawning
{
    public readonly struct SpawnEntityCommand : ICommand
    {
        public int Tick { get; }
        public string PrefabKey { get; }
        public float X { get; }
        public float Y { get; }

        public bool HasFixedId { get; }
        public GameEntityId FixedId { get; }

        public SpawnEntityCommand(int tick, string prefabKey, float x, float y)
        {
            Tick = tick;
            PrefabKey = prefabKey;
            X = x;
            Y = y;
            HasFixedId = false;
            FixedId = default;
        }

        public SpawnEntityCommand(int tick, string prefabKey, float x, float y, GameEntityId fixedId)
        {
            Tick = tick;
            PrefabKey = prefabKey;
            X = x;
            Y = y;
            HasFixedId = true;
            FixedId = fixedId;
        }
    }
}