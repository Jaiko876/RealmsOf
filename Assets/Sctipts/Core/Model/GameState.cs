using System.Collections.Generic;

namespace Game.Core.Model
{
    public sealed class GameState
    {
        public int Tick { get; private set; }

        private readonly Dictionary<PlayerId, PlayerState> _players = new Dictionary<PlayerId, PlayerState>();

        public IReadOnlyDictionary<PlayerId, PlayerState> Players => _players;

        public PlayerState GetOrCreatePlayer(PlayerId id)
        {
            PlayerState state;
            if (!_players.TryGetValue(id, out state))
            {
                state = new PlayerState(id);
                _players[id] = state;
            }

            return state;
        }

        public void SetTick(int tick)
        {
            Tick = tick;
        }

        public void AdvanceTick()
        {
            Tick++;
        }
    }
}
