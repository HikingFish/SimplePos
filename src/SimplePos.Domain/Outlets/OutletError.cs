using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Outlets;
public static class OutletError
{
    public static readonly Error OutletNameEmpty = new Error(
        "Outlets.OutletNameEmpty", "Outlet name cannot be empty", ErrorType.Validation);
    public static readonly Error OutletAddressNull = new Error(
        "Outlets.OutletAddressNull", "Outlet address cannot be null", ErrorType.Validation);
    public static readonly Error AlreadyActive = new Error(
        "Outlets.AlreadyActive", "Outlet is already active", ErrorType.Conflict);
    public static readonly Error AlreadyInactive = new Error(
        "Outlets.AlreadyInactive", "Outlet is already inactive", ErrorType.Conflict);
    public static readonly Error SoftDeleted = new Error(
        "Outlets.SoftDeleted", "Operation cannot be performed on a soft deleted outlet", ErrorType.Conflict);
    public static readonly Error OutletNotFound = new Error(
        "Outlets.OutletNotFound", "Outlet not found", ErrorType.NotFound);
    public static readonly Error CompanyMismatch = new Error(
        "Outlets.CompanyMismatch", "Outlet does not belong to the user's company", ErrorType.Validation);
    public static readonly Error AccessDenied = new Error(
        "Outlets.AccessDenied", "User is not authorized to access this outlet", ErrorType.Forbidden);
}