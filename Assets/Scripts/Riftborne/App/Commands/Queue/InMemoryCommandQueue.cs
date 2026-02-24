using System;
using System.Collections.Generic;
using Riftborne.Core.Input.Commands.Abstractions;

namespace Riftborne.App.Commands.Queue
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
            if (_byTick.Remove(tick, out var list))
            {
                return list;
            }

            return Array.Empty<ICommand>();
        }
    }
}