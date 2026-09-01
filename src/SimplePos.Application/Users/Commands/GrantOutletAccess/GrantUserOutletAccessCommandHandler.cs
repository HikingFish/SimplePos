using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Users;

namespace SimplePos.Application.Users.Commands.GrantOutletAccess;

public class GrantUserOutletAccessCommandHandler : ICommandHandler<GrantUserOutletAccessCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IOutletRepository _outletRepository;
    private readonly IUserOutletAccessRepository _userOutletAccessRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GrantUserOutletAccessCommandHandler(
        IUserRepository userRepository,
        IOutletRepository outletRepository,
        IUserOutletAccessRepository userOutletAccessRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _outletRepository = outletRepository;
        _userOutletAccessRepository = userOutletAccessRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(GrantUserOutletAccessCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(command.UserId);
        if (user == null)
        {
            return Result.Failure(UserError.AccountNotFound);
        }

        var outlet = await _outletRepository.GetOutletByIdAsync(command.OutletId);
        if (outlet == null)
        {
            return Result.Failure(OutletError.OutletNotFound);
        }

        if (outlet.CompanyId != user.CompanyId)
        {
            return Result.Failure(OutletError.CompanyMismatch);
        }

        var assignResult = user.AssignOutletAccess(command.OutletId);
        if (assignResult.IsFailure)
        {
            return assignResult;
        }

        var newAccess = user.UserOutletAccesses.First(uoa => uoa.OutletId == command.OutletId);
        _userOutletAccessRepository.AddUserOutletAccess(newAccess);

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
