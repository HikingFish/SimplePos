using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Taxes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    Guid ProductId,
    Guid CompanyId,
    Guid CategoryId,
    string SKU,
    string ProductName,
    decimal? CostPrice,
    decimal BasePrice,
    bool IsActive,
    bool SoftDeleted,
    Tax[] Taxes
) : ICommand;
