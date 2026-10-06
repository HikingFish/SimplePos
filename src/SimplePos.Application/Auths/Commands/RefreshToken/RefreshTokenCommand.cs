using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Auths.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Application.Auths.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<Result<AuthResponse>>;
