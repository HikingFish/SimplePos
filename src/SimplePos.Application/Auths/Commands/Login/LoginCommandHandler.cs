using SimplePos.Application.Abstractions.Identity;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Auths.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Application.Auths.Commands.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<AuthResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(command.Email, command.Password);
    }
}