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
}