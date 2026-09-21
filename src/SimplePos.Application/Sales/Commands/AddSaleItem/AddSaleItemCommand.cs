using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Sales.Commands.AddSaleItem;

public record AddSaleItemCommand(
    Guid SaleId,
    Guid UserId,
    Guid CompanyId,
    Guid ProductId,
    decimal Quantity,
    decimal? Price,
    decimal? Discount,
    string? Remarks,
    DateTime? FinancialDate
) : ICommand<Result<Guid>>;
