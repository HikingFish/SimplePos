namespace SimplePos.Application.Abstractions.Messaging;

public interface ICqrsDispatcher
{
    Task SendAsync<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : ICommand;
    Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken ct = default) where TCommand : ICommand<TResult>;
    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default);
}
