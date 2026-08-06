using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Payments;
public static class PaymentMethodError
{
    public static readonly Error CompanyIdEmpty = new Error(
        "PaymentMethod.CompanyIdEmpty", "Company ID cannot be empty", ErrorType.Validation);

    public static readonly Error NameEmpty = new Error(
        "PaymentMethod.NameEmpty", "Payment method name cannot be empty", ErrorType.Validation);

    public static readonly Error AlreadyActive = new Error(
        "PaymentMethod.AlreadyActive", "Payment method is already active", ErrorType.Conflict);

    public static readonly Error AlreadyInactive = new Error(
        "PaymentMethod.AlreadyInactive", "Payment method is already inactive", ErrorType.Conflict);

    public static readonly Error SoftDeleted = new Error(
        "PaymentMethod.SoftDeleted", "Operation cannot be performed on a soft-deleted payment method", ErrorType.Conflict);
}

