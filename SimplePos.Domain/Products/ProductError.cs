using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Products
{
    public static class ProductError
    {
        public static readonly Error ProductNameEmpty = new Error(
            "Products.ProductNameEmpty", "Product name cannot be empty");
        public static readonly Error SkuEmpty = new Error(
            "Products.SkuEmpty", "SKU cannot be empty");
        public static readonly Error CostPriceNegative = new Error(
            "Products.CostPriceNegative", "Cost price cannot be negative");
        public static readonly Error BasePriceNegative = new Error(
            "Products.BasePriceNegative", "Base price cannot be negative");
        public static readonly Error AlreadyActive = new Error(
            "Products.AlreadyActive", "Product is already active");
        public static readonly Error AlreadyInactive = new Error(
            "Products.AlreadyInactive", "Product is already inactive");
        public static readonly Error ProductTaxNull = new Error(
            "Products.ProductTaxNull", "Product tax cannot be null");
        public static readonly Error TaxAlreadyAssociated = new Error(
            "Products.TaxAlreadyAssociated", "This tax is already associated with the product");
        public static readonly Error TaxNotAssociated = new Error(
            "Products.TaxNotAssociated", "This tax is not associated with the product");
        public static readonly Error SoftDeleted = new Error(
            "Products.SoftDeleted", "Operation cannot be performed on a soft-deleted product");
    }
}
