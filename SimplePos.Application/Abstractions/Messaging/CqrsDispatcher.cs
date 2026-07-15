using Microsoft.Extensions.DependencyInjection;

namespace SimplePos.Application.Abstractions.Messaging;

public class CqrsDispatcher : ICqrsDispatcher
{
    private readonly IServiceProvider _provider;

    public CqrsDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Task SendAsync<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : ICommand
    {
        var handler = _provider.GetRequiredService<ICommandHandler<TCommand>>();
        return handler.HandleAsync(command, ct);
    }

    public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default)
    {
        // Because TQuery is not known at compile time via the method signature in the same way,
        // we use a little reflection to resolve the specific open generic type.
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        
        dynamic handler = _provider.GetRequiredService(handlerType);
        
        return handler.HandleAsync((dynamic)query, ct);
    }
}