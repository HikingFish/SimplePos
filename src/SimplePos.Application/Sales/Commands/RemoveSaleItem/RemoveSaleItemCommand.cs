using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Sales.Commands.RemoveSaleItem;

public record RemoveSaleItemCommand(Guid SaleId, Guid SaleItemId) : ICommand<Result>;
