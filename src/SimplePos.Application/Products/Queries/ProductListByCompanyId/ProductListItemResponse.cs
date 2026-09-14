namespace SimplePos.Application.Products.Queries.ProductListByCompanyId;

public record ProductListItemResponse(
    Guid ProductId,
    Guid? CategoryId,
    string SKU,
    string ProductName, 
    decimal BasePrice,
    bool IsActive,
    decimal? TotalTaxPercentage,
    decimal PriceWithTax
);
