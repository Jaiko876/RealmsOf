using System;
using System.Collections.Generic;
using Game.Core.Abstractions;

namespace Game.App.Commands
{
    public sealed class CommandDispatcher : ICommandDispatcher
    {
        private readonly Dictionary<Type, ICommandHandlerRegistration> _map;

        public CommandDispatcher(IEnumerable<ICommandHandlerRegistration> registrations)
        {
            _map = new Dictionary<Type, ICommandHandlerRegistration>();

            foreach (ICommandHandlerRegistration reg in registrations)
            {
                if (_map.ContainsKey(reg.CommandType))
                {
                    throw new InvalidOperationException("Duplicate handler registration for command: " + reg.CommandType.FullName);
                }

                _map.Add(reg.CommandType, reg);
            }
        }

        public void Dispatch(IReadOnlyList<ICommand> commands)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                ICommand cmd = commands[i];
                Type type = cmd.GetType();

                ICommandHandlerRegistration reg;
                if (!_map.TryGetValue(type, out reg))
                {
                    // В dev лучше падать сразу — иначе ты “теряешь” команды молча
                    throw new InvalidOperationException("No handler registered for command: " + type.FullName);
                }

                reg.Invoke(cmd);
            }
        }
    }
}
