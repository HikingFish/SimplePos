using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Products;
public static class ProductError
{
    public static readonly Error ProductNameEmpty = new Error(
        "Products.ProductNameEmpty", "Product name cannot be empty", ErrorType.Validation);
    public static readonly Error SkuEmpty = new Error(
        "Products.SkuEmpty", "SKU cannot be empty", ErrorType.Validation);
    public static readonly Error CostPriceNegative = new Error(
        "Products.CostPriceNegative", "Cost price cannot be negative", ErrorType.Validation);
    public static readonly Error BasePriceNegative = new Error(
        "Products.BasePriceNegative", "Base price cannot be negative", ErrorType.Validation);
    public static readonly Error AlreadyActive = new Error(
        "Products.AlreadyActive", "Product is already active", ErrorType.Conflict);
    public static readonly Error AlreadyInactive = new Error(
        "Products.AlreadyInactive", "Product is already inactive", ErrorType.Conflict);
    public static readonly Error ProductTaxNull = new Error(
        "Products.ProductTaxNull", "Product tax cannot be null", ErrorType.Validation);
    public static readonly Error TaxAlreadyAssociated = new Error(
        "Products.TaxAlreadyAssociated", "This tax is already associated with the product", ErrorType.Conflict);
    public static readonly Error TaxNotAssociated = new Error(
        "Products.TaxNotAssociated", "This tax is not associated with the product", ErrorType.NotFound);
    public static readonly Error SoftDeleted = new Error(
        "Products.SoftDeleted", "Operation cannot be performed on a soft-deleted product", ErrorType.Conflict);
    public static readonly Error CompanyIdEmpty = new Error(
        "Products.CompanyIdEmpty", "Company Id cannot be empty", ErrorType.Validation);
    public static readonly Error CategoryIdEmpty = new Error(
        "Products.CategoryIdEmpty", "Category Id cannot be empty", ErrorType.Validation);
    public static readonly Error ProductNotExist = new Error(
        "Products.ProductNotExist", "Product does not exist", ErrorType.NotFound);
}

