using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Users;

public class UserOutletAccess
{
    public Guid UserId { get; private set; }
    public Guid OutletId { get; private set; }

    private UserOutletAccess() { }

    private UserOutletAccess(Guid userId, Guid outletId)
    {
        UserId = userId;
        OutletId = outletId;
    }

    public static Result<UserOutletAccess> Create(Guid userId, Guid outletId)
    {
        if (userId == Guid.Empty)
        {
            return Result<UserOutletAccess>.Failure(UserOutletAccessError.UserIdEmpty);
        }

        if (outletId == Guid.Empty)
        {
            return Result<UserOutletAccess>.Failure(UserOutletAccessError.OutletIdEmpty);
        }

        return Result<UserOutletAccess>.Success(new UserOutletAccess(userId, outletId));
    }
}
