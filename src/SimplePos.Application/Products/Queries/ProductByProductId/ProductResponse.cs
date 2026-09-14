using SimplePos.Domain.Products;
using SimplePos.Domain.Taxes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Products.Queries.ProductByProductId;

public record ProductResponse(
    Guid ProductId,
    Guid? CategoryId,
    string SKU,
    string ProductName,
    decimal? CostPrice,
    decimal BasePrice,
    bool IsActive,
    string CategoryName,
    List<Tax>? Taxes
);
