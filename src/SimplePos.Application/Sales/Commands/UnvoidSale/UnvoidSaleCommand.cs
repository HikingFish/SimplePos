using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Sales.Commands.UnvoidSale;

public record UnvoidSaleCommand(Guid SaleId) : ICommand<Result>;
