using SimplePos.Application.Abstractions.Identity;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Users;

namespace SimplePos.Application.Auths.Commands.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, Result<string>>
{
    IUserRepository _userRepository;
    ITokenProvider _tokenProvider;
    IIdentityService _identityService;

    public LoginCommandHandler(IUserRepository userRepository, ITokenProvider tokenProvider, IIdentityService identityService)
    {
        _userRepository = userRepository;
        _tokenProvider = tokenProvider;
        _identityService = identityService;
    }

    public async Task<Result<string>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(command.Email, command.Password);
    }
}