using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Taxes
{
    public static class TaxError
    {
        public static readonly Error TaxNameEmpty = new Error(
            "Tax.TaxNameEmpty", "Tax name cannot be empty.");
        public static readonly Error TaxRateNegative = new Error(
            "Tax.TaxRateNegative", "Tax rate cannot be negative.");
        public static readonly Error AlreadyDeactivated = new Error(
            "Tax.AlreadyDeactivated", "Tax is already deactivated.");
        public static readonly Error AlreadyActivated = new Error(
            "Tax.AlreadyActivated", "Tax is already activated.");
    }
}
