using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Game.Domain.Abstractions;

namespace Game.Infrastructure.Commands
{

    public sealed class CachedCommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        // commandType -> action(ICommand)
        private readonly ConcurrentDictionary<Type, Action<ICommand>> _cache = new();

        public CachedCommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Dispatch(System.Collections.Generic.IReadOnlyList<ICommand> commands)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                var cmd = commands[i];
                var action = _cache.GetOrAdd(cmd.GetType(), BuildDispatchAction);
                action(cmd);
            }
        }

        private Action<ICommand> BuildDispatchAction(Type commandType)
        {
            // ICommandHandler<TCommand>
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);

            // (ICommand cmd) => ((ICommandHandler<T>)sp.GetService(handlerType)).Handle((T)cmd)
            var cmdParam = Expression.Parameter(typeof(ICommand), "cmd");

            var spConst = Expression.Constant(_serviceProvider);

            var getServiceMethod = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService))!;
            var handlerObj = Expression.Call(spConst, getServiceMethod, Expression.Constant(handlerType));

            var handlerCast = Expression.Convert(handlerObj, handlerType);

            var handleMethod = handlerType.GetMethod("Handle")!;
            var cmdCast = Expression.Convert(cmdParam, commandType);

            var callHandle = Expression.Call(handlerCast, handleMethod, cmdCast);

            // Если handler не зарегистрирован — просто no-op (можно сделать throw, если хочешь строгий режим)
            // Чтобы не городить try/catch на hotpath, делаем проверку на null в скомпилированном делегате.
            var handlerObjVar = Expression.Variable(typeof(object), "h");
            var assign = Expression.Assign(handlerObjVar, handlerObj);

            var ifNull = Expression.IfThen(
                Expression.Equal(handlerObjVar, Expression.Constant(null)),
                Expression.Return(Expression.Label())
            );

            // Но Return без лейбла неудобен — проще: if (handler==null) return;
            var retLabel = Expression.Label();

            var ifNullRet = Expression.IfThen(
                Expression.Equal(handlerObjVar, Expression.Constant(null)),
                Expression.Goto(retLabel)
            );

            var callHandleWithVar = Expression.Call(
                Expression.Convert(handlerObjVar, handlerType),
                handleMethod,
                cmdCast
            );

            var body = Expression.Block(
                new[] { handlerObjVar },
                assign,
                ifNullRet,
                callHandleWithVar,
                Expression.Label(retLabel)
            );

            return Expression.Lambda<Action<ICommand>>(body, cmdParam).Compile();
        }
    }
}