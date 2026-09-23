using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.PaymentMethods.Commands.CreatePaymentMethod;

public record CreatePaymentMethodCommand(Guid CompanyId, string Name) : ICommand<Result<Guid>>;
