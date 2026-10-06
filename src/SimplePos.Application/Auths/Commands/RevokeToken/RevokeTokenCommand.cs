using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Application.Auths.Commands.RevokeToken;

public record RevokeTokenCommand(string RefreshToken) : ICommand<Result>;
