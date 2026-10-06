using SimplePos.Application.Abstractions.Identity;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Application.Auths.Commands.RevokeToken;

public class RevokeTokenCommandHandler : ICommandHandler<RevokeTokenCommand, Result>
{
    private readonly IIdentityService _identityService;

    public RevokeTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> HandleAsync(RevokeTokenCommand command, CancellationToken cancellationToken)
    {
        return await _identityService.RevokeTokenAsync(command.RefreshToken, cancellationToken);
    }
}
