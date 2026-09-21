using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;

namespace SimplePos.Application.Sales.Commands.UpdateSaleItem;

public record UpdateSaleItemCommand(
    Guid SaleId,
    Guid SaleItemId,
    decimal Quantity,
    decimal UnitPrice,
    decimal UnitDiscount,
    string? Remark,
    List<Guid>? TaxIds,
    Guid CompanyId
) : ICommand<Result>;
