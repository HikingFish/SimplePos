using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Sales;
public static class SaleItemError
{
    public static readonly Error SaleIdEmpty = new Error(
        "SaleItem.SaleIdEmpty", "Sale ID cannot be empty.", ErrorType.Validation);

    public static readonly Error ProductIdEmpty = new Error(
        "SaleItem.ProductIdEmpty", "Product ID cannot be empty.", ErrorType.Validation);

    public static readonly Error SkuEmpty = new Error(
        "SaleItem.SkuEmpty", "Product SKU cannot be empty.", ErrorType.Validation);

    public static readonly Error ProductNameEmpty = new Error(
        "SaleItem.ProductNameEmpty", "Product name cannot be empty.", ErrorType.Validation);

    public static readonly Error QuantityZero = new Error(
        "SaleItem.QuantityZero", "Quantity cannot be zero.", ErrorType.Validation);

    public static readonly Error UnitPriceNegative = new Error(
        "SaleItem.UnitPriceNegative", "Unit price cannot be negative.", ErrorType.Validation);

    public static readonly Error DiscountNegative = new Error(
        "SaleItem.DiscountNegative", "Discount cannot be negative.", ErrorType.Validation);

    public static readonly Error TaxRateNegative = new Error(
        "SaleItem.TaxRateNegative", "Tax rate percentage cannot be negative.", ErrorType.Validation);

    public static readonly Error DiscountExceedsTotal = new Error(
        "SaleItem.DiscountExceedsTotal", "Discount cannot exceed line amount.", ErrorType.Validation);
    public static readonly Error UnitDiscountNegative = new Error(
        "SaleItem.UnitDiscountNegative", "Unit discount cannot be negative.", ErrorType.Validation);
    public static readonly Error AddedByUserIdEmpty = new Error(
        "SaleItem.AddedByUserIdEmpty", "Added by user ID cannot be empty.", ErrorType.Validation);
    public static readonly Error VoidedByUserIdEmpty = new Error(
        "SaleItem.VoidedByUserIdEmpty", "Voided by user ID cannot be empty.", ErrorType.Validation);
    public static readonly Error Void = new Error(
        "SaleItem.Void", "Operation cannot be performed on a void sale item.", ErrorType.Conflict);
    public static readonly Error AlreadyVoid = new Error(
        "SaleItem.AlreadyVoid", "Sale item is already void.", ErrorType.Conflict);
    public static readonly Error NotVoid = new Error(
        "SaleItem.NotVoid", "Operation cannot be performed on a not void sale item.", ErrorType.Conflict);
}

