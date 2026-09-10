using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Taxes;
public static class TaxError
{
    public static readonly Error TaxNameEmpty = new Error(
        "Tax.TaxNameEmpty", "Tax name cannot be empty.", ErrorType.Validation);
    public static readonly Error TaxRateNegative = new Error(
        "Tax.TaxRateNegative", "Tax rate cannot be negative.", ErrorType.Validation);
    public static readonly Error AlreadyDeactivated = new Error(
        "Tax.AlreadyDeactivated", "Tax is already deactivated.", ErrorType.Conflict);
    public static readonly Error AlreadyActivated = new Error(
        "Tax.AlreadyActivated", "Tax is already activated.", ErrorType.Conflict);
    public static readonly Error SoftDeleted = new Error(
        "Tax.SoftDeleted", "Operation cannot be performed on a soft deleted tax rate.", ErrorType.Conflict);
    public static readonly Error NotExist = new Error(
        "Tax.NotExist", "Tax does not exist.", ErrorType.NotFound);
}

