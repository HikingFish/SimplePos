namespace SimplePos.Application.Abstractions.Messaging;
//to mark command and query
//payloads
public interface ICommand { }

public interface ICommand<TResult>{ }

//in to ensure it is only given as input and not returned as output
//accepts only commands that implement ICommand interface
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}