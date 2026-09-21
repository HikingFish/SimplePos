using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Sales.Commands.VoidSale;

public record VoidSaleCommand(Guid SaleId, Guid UserId) : ICommand<Result>;
