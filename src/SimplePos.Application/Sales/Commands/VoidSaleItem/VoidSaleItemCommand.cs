using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Sales.Commands.VoidSaleItem;

public record VoidSaleItemCommand(Guid SaleId, Guid SaleItemId, Guid UserId) : ICommand<Result>;
