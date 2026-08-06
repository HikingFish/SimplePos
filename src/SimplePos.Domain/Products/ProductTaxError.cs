using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Products;

public static class ProductTaxError
{
    public static readonly Error ProductIdEmpty = new Error(
        "ProductTax.ProductIdEmpty", "Product ID cannot be empty.", ErrorType.Validation);

    public static readonly Error TaxIdEmpty = new Error(
        "ProductTax.TaxIdEmpty", "Tax ID cannot be empty.", ErrorType.Validation);
}
