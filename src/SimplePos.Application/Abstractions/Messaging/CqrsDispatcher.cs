using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace SimplePos.Application.Abstractions.Messaging;

public class CqrsDispatcher : ICqrsDispatcher
{
    private readonly IServiceProvider _provider;
    private static readonly ConcurrentDictionary<Type, object> _queryWrappers = new();

    public CqrsDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Task SendAsync<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : ICommand
    {
        var handler = _provider.GetRequiredService<ICommandHandler<TCommand>>();
        return handler.HandleAsync(command, ct);
    }

    public Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken ct = default) where TCommand : ICommand<TResult>
    {
        var handler = _provider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return handler.HandleAsync(command, ct);
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
}