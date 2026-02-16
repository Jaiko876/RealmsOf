using System;
using System.Collections.Generic;
using Game.Core.Abstractions;

namespace Game.App.Commands
{

    public sealed class InMemoryCommandQueue : ICommandQueue
    {
        private readonly Dictionary<int, List<ICommand>> _byTick = new();

        public void Enqueue(ICommand command)
        {
            if (!_byTick.TryGetValue(command.Tick, out var list))
            {
                list = new List<ICommand>(8);
                _byTick[command.Tick] = list;
            }

            list.Add(command);
        }

        public IReadOnlyList<ICommand> DequeueAllForTick(int tick)
        {
            if (_byTick.TryGetValue(tick, out var list))
            {
                _byTick.Remove(tick);
                return list;
            }

            return Array.Empty<ICommand>();
        }
    }
}