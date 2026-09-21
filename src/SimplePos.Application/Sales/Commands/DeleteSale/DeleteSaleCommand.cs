using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Sales.Commands.DeleteSale;

public record DeleteSaleCommand(Guid SaleId) : ICommand<Result>;
