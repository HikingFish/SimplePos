using SimplePos.Application.Abstractions.Identity;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Auths.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Application.Auths.Commands.RefreshToken;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<AuthResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        return await _identityService.RefreshTokenAsync(command.RefreshToken, cancellationToken);
    }
}
