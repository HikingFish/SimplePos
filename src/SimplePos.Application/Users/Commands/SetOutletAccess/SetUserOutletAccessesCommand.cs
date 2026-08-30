using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Users;

namespace SimplePos.Application.Users.Commands.SetOutletAccess;

public record SetUserOutletAccessesCommand(Guid UserId, List<Guid> OutletIds) : ICommand<Result>;

public class SetUserOutletAccessesCommandHandler : ICommandHandler<SetUserOutletAccessesCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IOutletRepository _outletRepository;
    private readonly IUserOutletAccessRepository _userOutletAccessRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetUserOutletAccessesCommandHandler(
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

    public async Task<Result> HandleAsync(SetUserOutletAccessesCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(command.UserId);
        if (user == null)
        {
            return Result.Failure(UserError.AccountNotFound);
        }

        var uniqueOutletIds = command.OutletIds.Distinct().ToList();

        // Validate all outlets
        foreach (var outletId in uniqueOutletIds)
        {
            if (outletId == Guid.Empty)
            {
                return Result.Failure(UserOutletAccessError.OutletIdEmpty);
            }

            var outlet = await _outletRepository.GetOutletByIdAsync(outletId);
            if (outlet == null)
            {
                return Result.Failure(OutletError.OutletNotFound);
            }

            if (outlet.CompanyId != user.CompanyId)
            {
                return Result.Failure(OutletError.CompanyMismatch);
            }
        }

        // Remove existing from DB
        var existingAccesses = await _userOutletAccessRepository.GetUserOutletAccessesByUserIdAsync(command.UserId);
        if (existingAccesses.Count > 0)
        {
            _userOutletAccessRepository.DeleteUserOutletAccesses(existingAccesses);
        }

        // Clear and re-add in domain aggregate
        user.ClearOutletAccesses();
        foreach (var outletId in uniqueOutletIds)
        {
            var assignResult = user.AssignOutletAccess(outletId);
            if (assignResult.IsFailure)
            {
                return assignResult;
            }
            var newAccess = user.UserOutletAccesses.First(uoa => uoa.OutletId == outletId);
            _userOutletAccessRepository.AddUserOutletAccess(newAccess);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
