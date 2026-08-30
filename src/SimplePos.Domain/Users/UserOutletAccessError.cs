using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Users;

public static class UserOutletAccessError
{
    public static readonly Error OutletIdEmpty = new Error(
        "UserOutletAccess.OutletIdEmpty", "Outlet ID cannot be empty", ErrorType.Validation);

    public static readonly Error UserIdEmpty = new Error(
        "UserOutletAccess.UserIdEmpty", "User ID cannot be empty", ErrorType.Validation);

    public static readonly Error OutletAlreadyAssigned = new Error(
        "UserOutletAccess.OutletAlreadyAssigned", "Outlet access is already assigned to this user", ErrorType.Conflict);

    public static readonly Error OutletNotAssigned = new Error(
        "UserOutletAccess.OutletNotAssigned", "Outlet access is not assigned to this user", ErrorType.NotFound);
}
