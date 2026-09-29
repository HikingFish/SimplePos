using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace SimplePos.Application.Abstractions.Messaging;

public class CqrsDispatcher : ICqrsDispatcher
{
    private readonly IServiceProvider _provider;
    private static readonly ConcurrentDictionary<Type, object> _queryWrappers = new();
    private static readonly ConcurrentDictionary<Type, object> _commandWrappers = new();

    public CqrsDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Task SendAsync<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : ICommand
    {
        var wrapper = (CommandHandlerWrapper)_commandWrappers.GetOrAdd(
            typeof(TCommand),
            static t => Activator.CreateInstance(typeof(CommandHandlerWrapperImp<>).MakeGenericType(t))!
            );
        return wrapper.HandleAsync(command, _provider, ct);
    }

    public Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken ct = default) where TCommand : ICommand<TResult>
    {
        var wrapper = (CommandHandlerWrapper<TResult>)_commandWrappers.GetOrAdd(
            command.GetType(), static t => Activator.CreateInstance(typeof(CommandHandlerWrapperImp<,>).MakeGenericType(t, typeof(TResult)))!
            );
        return wrapper.HandleAsync(command, _provider, ct);
    }

    public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default)
    {
        var queryType = query.GetType();

        var wrapper = (QueryHandlerWrapper<TResult>)_queryWrappers.GetOrAdd(
            queryType,
            static t => Activator.CreateInstance(typeof(QueryHandlerWrapperImpl<,>).MakeGenericType(t, typeof(TResult)))!
            );

        return wrapper.HandleAsync(query, _provider, ct);
    }

    private abstract class QueryHandlerWrapper<TRes>
    {
        public abstract Task<TRes> HandleAsync(IQuery<TRes> query, IServiceProvider provider, CancellationToken ct);
    }

    private sealed class QueryHandlerWrapperImpl<TQuery, TRes> : QueryHandlerWrapper<TRes> where TQuery : IQuery<TRes>
    {
        public override Task<TRes> HandleAsync(IQuery<TRes> query, IServiceProvider provider, CancellationToken ct)
        {
            var handler = provider.GetRequiredService<IQueryHandler<TQuery, TRes>>();
            return handler.HandleAsync((TQuery)query, ct);
        }
    }

    private abstract class CommandHandlerWrapper
    {
        public abstract Task HandleAsync(ICommand command, IServiceProvider provider, CancellationToken ct);
    }

    private sealed class CommandHandlerWrapperImp<TCommand> : CommandHandlerWrapper where TCommand : ICommand
    {
        public override Task HandleAsync(ICommand command, IServiceProvider provider, CancellationToken ct)
        {
            var handler = provider.GetRequiredService<ICommandHandler<TCommand>>();
            return handler.HandleAsync((TCommand)command, ct);
        }
    }

    private abstract class CommandHandlerWrapper<TResult>
    {
        public abstract Task<TResult> HandleAsync(ICommand<TResult> command, IServiceProvider provider, CancellationToken ct);
    }

    private sealed class CommandHandlerWrapperImp<TCommand, TResult> : CommandHandlerWrapper<TResult> where TCommand : ICommand<TResult>
    {
        public override Task<TResult> HandleAsync(ICommand<TResult> command, IServiceProvider provider, CancellationToken ct)
        {
            var handler = provider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
            return handler.HandleAsync((TCommand)command, ct);
        }
    }
} 