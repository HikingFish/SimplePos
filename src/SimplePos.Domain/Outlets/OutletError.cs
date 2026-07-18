using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Outlets;
public static class OutletError
{
    public static readonly Error OutletNameEmpty = new Error(
        "Outlets.OutletNameEmpty", "Outlet name cannot be empty");
    public static readonly Error OutletAddressNull = new Error(
        "Outlets.OutletAddressNull", "Outlet address cannot be null");
    public static readonly Error AlreadyActive = new Error(
        "Outlets.AlreadyActive", "Outlet is already active");
    public static readonly Error AlreadyInactive = new Error(
        "Outlets.AlreadyInactive", "Outlet is already inactive");
    public static readonly Error SoftDeleted = new Error(
        "Outlets.SoftDeleted", "Operation cannot be performed on a soft deleted outlet");
}