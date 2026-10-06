using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Auths.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Application.Auths.Commands.Login;

public record LoginCommand(string Email, string Password) : ICommand<Result<AuthResponse>>;