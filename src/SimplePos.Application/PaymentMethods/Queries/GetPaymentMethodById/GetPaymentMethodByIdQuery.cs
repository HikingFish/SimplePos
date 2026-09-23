using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.PaymentMethods.Common;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.PaymentMethods.Queries.GetPaymentMethodById;

public record GetPaymentMethodByIdQuery(Guid PaymentMethodId, Guid CompanyId) : IQuery<Result<PaymentMethodResponse>>;
