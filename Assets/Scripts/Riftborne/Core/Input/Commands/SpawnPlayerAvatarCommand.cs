using Riftborne.Core.Input.Commands.Abstractions;
using Riftborne.Core.Model;

namespace Riftborne.Core.Input.Commands
{
    public readonly struct SpawnPlayerAvatarCommand : ICommand
    {
        public int Tick { get; }
        public PlayerId PlayerId { get; }

        public string PrefabKey { get; }
        public float X { get; }
        public float Y { get; }

        public bool HasFixedId { get; }
        public GameEntityId FixedId { get; }

        public SpawnPlayerAvatarCommand(int tick, PlayerId playerId, string prefabKey, float x, float y)
        {
            Tick = tick;
            PlayerId = playerId;
            PrefabKey = prefabKey;
            X = x;
            Y = y;
            HasFixedId = false;
            FixedId = default;
        }

        public SpawnPlayerAvatarCommand(int tick, PlayerId playerId, string prefabKey, float x, float y, GameEntityId fixedId)
        {
            Tick = tick;
            PlayerId = playerId;
            PrefabKey = prefabKey;
            X = x;
            Y = y;
            HasFixedId = true;
            FixedId = fixedId;
        }
    }
}