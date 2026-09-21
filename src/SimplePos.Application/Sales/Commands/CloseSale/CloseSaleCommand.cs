using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Sales.Commands.CloseSale;

public record CloseSaleCommand(Guid SaleId, Guid UserId) : ICommand<Result>;
