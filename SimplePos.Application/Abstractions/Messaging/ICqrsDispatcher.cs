namespace SimplePos.Application.Abstractions.Messaging;

public interface ICqrsDispatcher
{
    Task SendAsync<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : ICommand;
    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default);
}
