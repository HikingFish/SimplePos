using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.PaymentMethods.Commands.DeactivatePaymentMethod;

public record DeactivatePaymentMethodCommand(Guid PaymentMethodId, Guid CompanyId) : ICommand<Result>;
