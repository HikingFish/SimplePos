using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Sales.Commands.UnvoidSaleItem;

public record UnvoidSaleItemCommand(Guid SaleId, Guid SaleItemId) : ICommand<Result>;
