using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Taxes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    Guid CompanyId,
    Guid? CategoryId,
    string SKU,
    string ProductName,
    decimal? CostPrice,
    decimal BasePrice,
    Guid[]? Taxes
) : ICommand<Result<Guid>>;
