using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Sales.Commands.RemoveSalePayment;

public record RemoveSalePaymentCommand(Guid SaleId, Guid PaymentId) : ICommand<Result>;
