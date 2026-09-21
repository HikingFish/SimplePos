using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Sales.Commands.UpdateSaleItemQuantity;

public record UpdateSaleItemQuantityCommand(Guid SaleId, Guid SaleItemId, decimal Quantity) : ICommand<Result>;
