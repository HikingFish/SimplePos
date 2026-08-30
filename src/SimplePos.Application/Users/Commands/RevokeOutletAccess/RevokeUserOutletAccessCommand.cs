using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Users;

namespace SimplePos.Application.Users.Commands.RevokeOutletAccess;

public record RevokeUserOutletAccessCommand(Guid UserId, Guid OutletId) : ICommand<Result>;

public class RevokeUserOutletAccessCommandHandler : ICommandHandler<RevokeUserOutletAccessCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserOutletAccessRepository _userOutletAccessRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeUserOutletAccessCommandHandler(
        IUserRepository userRepository,
        IUserOutletAccessRepository userOutletAccessRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userOutletAccessRepository = userOutletAccessRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(RevokeUserOutletAccessCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(command.UserId);
        if (user == null)
        {
            return Result.Failure(UserError.AccountNotFound);
        }

        var access = await _userOutletAccessRepository.GetUserOutletAccessAsync(command.UserId, command.OutletId);
        if (access == null)
        {
            return Result.Failure(UserOutletAccessError.OutletNotAssigned);
        }

        user.RevokeOutletAccess(command.OutletId);
        _userOutletAccessRepository.DeleteUserOutletAccess(access);

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
