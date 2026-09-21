using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Sales.Commands.AddSalePayment;

public record AddSalePaymentCommand(
    Guid SaleId,
    Guid PaymentMethodId,
    Guid ProcessedByUserId,
    decimal AmountPaid,
    DateTime? PaymentDate,
    string? ReferenceNumber
) : ICommand<Result<Guid>>;
