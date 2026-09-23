using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.PaymentMethods.Commands.ActivatePaymentMethod;

public record ActivatePaymentMethodCommand(Guid PaymentMethodId, Guid CompanyId) : ICommand<Result>;
