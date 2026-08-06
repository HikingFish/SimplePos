using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Sales;

public static class SaleItemTaxError
{
    public static readonly Error SaleItemIdEmpty = new Error(
        "SaleItemTax.SaleItemIdEmpty", "Sale item ID cannot be empty.", ErrorType.Validation);

    public static readonly Error TaxIdEmpty = new Error(
        "SaleItemTax.TaxIdEmpty", "Tax ID cannot be empty.", ErrorType.Validation);

    public static readonly Error TaxNameEmpty = new Error(
        "SaleItemTax.TaxNameEmpty", "Tax name cannot be empty.", ErrorType.Validation);

    public static readonly Error TaxRateNegative = new Error(
        "SaleItemTax.TaxRateNegative", "Tax rate percentage cannot be negative.", ErrorType.Validation);
}
