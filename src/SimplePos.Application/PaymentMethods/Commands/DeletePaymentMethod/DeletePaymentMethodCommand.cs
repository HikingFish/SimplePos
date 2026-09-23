using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.PaymentMethods.Commands.DeletePaymentMethod;

public record DeletePaymentMethodCommand(Guid PaymentMethodId, Guid CompanyId) : ICommand<Result>;
