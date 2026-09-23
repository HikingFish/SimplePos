using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.PaymentMethods.Commands.UpdatePaymentMethod;

public record UpdatePaymentMethodCommand(Guid PaymentMethodId, Guid CompanyId, string Name) : ICommand<Result>;
