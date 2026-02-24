using System;
using System.Collections.Generic;
using Riftborne.App.Commands.Handlers;
using Riftborne.Core.Input.Commands.Abstractions;

namespace Riftborne.App.Commands.Dispatching
{
    public sealed class CommandDispatcher : ICommandDispatcher
    {
        private readonly Dictionary<Type, List<ICommandHandlerRegistration>> _map;

        public CommandDispatcher(IEnumerable<ICommandHandlerRegistration> registrations)
        {
            _map = new Dictionary<Type, List<ICommandHandlerRegistration>>();

            foreach (ICommandHandlerRegistration reg in registrations)
            {
                if (!_map.TryGetValue(reg.CommandType, out var list))
                {
                    list = new List<ICommandHandlerRegistration>(4);
                    _map.Add(reg.CommandType, list);
                }

                list.Add(reg);
            }
        }

        public void Dispatch(IReadOnlyList<ICommand> commands)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                var cmd = commands[i];
                var type = cmd.GetType();

                if (!_map.TryGetValue(type, out var handlers) || handlers.Count == 0)
                    throw new InvalidOperationException("No handler registered for command: " + type.FullName);

                for (int h = 0; h < handlers.Count; h++)
                    handlers[h].Invoke(cmd);
            }
        }
    }
}