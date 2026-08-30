using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Outlets;

public static class OutletProductAvailabilityError
{
    public static readonly Error OutletIdEmpty = new Error(
        "OutletProductAvailability.OutletIdEmpty", "Outlet ID cannot be empty", ErrorType.Validation);

    public static readonly Error ProductIdEmpty = new Error(
        "OutletProductAvailability.ProductIdEmpty", "Product ID cannot be empty", ErrorType.Validation);

    public static readonly Error UserIdEmpty = new Error(
        "OutletProductAvailability.UserIdEmpty", "User ID cannot be empty", ErrorType.Validation);

    public static readonly Error AlreadyAvailable = new Error(
        "OutletProductAvailability.AlreadyAvailable", "Product is already marked as available", ErrorType.Conflict);

    public static readonly Error AlreadyUnavailable = new Error(
        "OutletProductAvailability.AlreadyUnavailable", "Product is already marked as unavailable", ErrorType.Conflict);
}
